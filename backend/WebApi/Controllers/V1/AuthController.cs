using Microsoft.AspNetCore.Mvc;
using OtpNet;
using QRCoder;
using WebApi.Dtos;
using WebApi.Repositories;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : ControllerBase
    {
        private static readonly string Issuer = Uri.EscapeDataString("TimedBasedOTPApp");
        private static readonly string DefaultUser = Uri.EscapeDataString("defaultUser");

        private readonly SecuritySettingsRepository repository;

        public AuthController(SecuritySettingsRepository repository)
        {
            this.repository = repository;
        }

        private CancellationToken CancellationToken => this.HttpContext.RequestAborted;

        [HttpGet]
        public async Task<ActionResult<Dtos.SecuritySettings>> Get()
        {
            var settings = await this.repository.GetFirstAsync(this.CancellationToken);

            if (settings == null)
            {
                return this.NotFound();
            }

            return this.Ok(settings);
        }

        [HttpPost("totp/enable")]
        public async Task<IActionResult> Enable()
        {
            var settings = await this.repository.GetFirstAsync(this.CancellationToken);

            if (settings != null)
            {
                return this.BadRequest(new ErrorResponse
                {
                    ErrorMessage = "You have already enabled the TOTP. Remove from database if want to reset.",
                    ReferenceId = Guid.NewGuid().ToString(),
                });
            }

            var secretKey = KeyGeneration.GenerateRandomKey();
            var base32SecretKey = Base32Encoding.ToString(secretKey);

            var otpUri = $"otpauth://totp/{Issuer}:{DefaultUser}?secret={base32SecretKey}&issuer={Issuer}&digits=6&period=30";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(otpUri, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(10);

            await this.repository.UpsertAsync(new Models.SecuritySettings
            {
                Id = "fixedId",
                CreatedAt = DateTime.UtcNow,
                IsTotpEnabled = false,
                OtpSecret = base32SecretKey,
            }, this.CancellationToken);

            return this.File(qrCodeImage, "images/png");
        }

        [HttpPost("totp/validate/{code}")]
        public async Task<IActionResult> Validate(string code)
        {
            var settings = await this.repository.GetFirstAsync(this.CancellationToken);

            if (settings is null)
            {
                return this.BadRequest(new ErrorResponse
                {
                    ErrorMessage = "You have not enabled the OTP yet.",
                    ReferenceId = Guid.NewGuid().ToString(),
                });
            }

            var totp = new Totp(Base32Encoding.ToBytes(settings.OtpSecret));
            var isValid = totp.VerifyTotp(
                code,
                out var timeStepMatched,
                VerificationWindow.RfcSpecifiedNetworkDelay);

            if (!isValid)
            {
                return this.Unauthorized(new ErrorResponse
                {
                    ErrorMessage = "Invalid OTP code provided.",
                    ReferenceId = Guid.NewGuid().ToString(),
                });
            }

            // Can we not call this every time
            settings.IsTotpEnabled = true;

            await this.repository.UpsertAsync(settings, this.CancellationToken);

            return this.Ok();
        }
    }
}
