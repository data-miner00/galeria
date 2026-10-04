using Microsoft.AspNetCore.Mvc;
using Core.Models;
using Core.Repositories;
using WebApi.Models;
using WebApi.Extensions;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserSettingsController : ControllerBase
    {
        private readonly UserSettingsRepository repository;

        public UserSettingsController(UserSettingsRepository repository)
        {
            this.repository = repository;
        }

        private CancellationToken CancellationToken => this.HttpContext.RequestAborted;

        [HttpGet]
        public async Task<ActionResult<UserSettings>> GetSettings()
        {
            var settings = await this.repository.GetFirstAsync(this.CancellationToken);

            if (settings == null)
            {
                return Ok(new UserSettings
                {
                    NoOfColumns = 5,
                    LayoutType = "masonry",
                });
            }

            return Ok(settings);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateSettings([FromBody] UpdateUserSettingsRequest request)
        {
            if (!request.HasAnyNonNullProperty())
            {
                return this.BadRequest(new ErrorResponse
                {
                    ErrorMessage = "The request needs to have at least 1 property populated.",
                });
            }

            var settings = await this.repository.GetFirstAsync(this.CancellationToken);
            settings ??= new UserSettings { Id = "UserProfileId", CreatedAt = DateTime.UtcNow };

            UpdateSettingsFromRequest(settings, request);

            await this.repository.UpsertAsync(settings, this.CancellationToken);

            return this.Ok(settings);
        }

        private static void UpdateSettingsFromRequest(UserSettings settings, UpdateUserSettingsRequest request)
        {
            if (request.NoOfColumns.HasValue)
            {
                settings.NoOfColumns = request.NoOfColumns.Value;
            }

            if (request.Watermark is not null)
            {
                settings.Watermark = string.IsNullOrWhiteSpace(request.Watermark) ? null : request.Watermark;
            }

            if (request.LayoutType is not null)
            {
                settings.LayoutType = request.LayoutType;
            }

            if (request.RecycleBinRetentionDays.HasValue)
            {
                settings.RecycleBinRetentionDays = request.RecycleBinRetentionDays.Value;
            }
        }
    }
}
