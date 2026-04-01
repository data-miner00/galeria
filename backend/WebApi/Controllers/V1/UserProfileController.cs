using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserProfileController : ControllerBase
    {
        private readonly UserProfileRepository repository;

        public UserProfileController(UserProfileRepository repository)
        {
            this.repository = repository;
        }

        private CancellationToken CancellationToken => this.HttpContext.RequestAborted;

        [HttpGet]
        public async Task<ActionResult<UserProfile>> GetProfile()
        {
            var profile = (await this.repository.GetAllAsync(this.CancellationToken)).FirstOrDefault();

            return this.Ok(profile ?? new UserProfile());
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
        {
            if (!request.HasAnyNonNullProperty())
            {
                return this.BadRequest(new ErrorResponse
                {
                    ErrorMessage = "The request needs to have at least 1 property populated.",
                });
            }

            var profile = (await this.repository.GetAllAsync(this.CancellationToken)).FirstOrDefault();
            profile ??= new UserProfile { Id = "UserProfileId", CreatedAt = DateTime.UtcNow };

            UpdateProfileFromRequest(profile, request);

            await this.repository.UpsertAsync(profile, this.CancellationToken);

            return this.Ok(profile);
        }

        private static void UpdateProfileFromRequest(UserProfile profile, UpdateUserProfileRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                profile.Username = request.Username;
            }

            if (!string.IsNullOrWhiteSpace(request.FirstName))
            {
                profile.FirstName = request.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(request.LastName))
            {
                profile.LastName = request.LastName;
            }

            if (!string.IsNullOrWhiteSpace(request.AvatarImage))
            {
                profile.AvatarImage = request.AvatarImage;
            }

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                profile.Email = request.Email;
            }

            if (!string.IsNullOrWhiteSpace(request.Bio))
            {
                profile.Bio = request.Bio;
            }

            if (!string.IsNullOrWhiteSpace(request.WebsiteUrl))
            {
                profile.WebsiteUrl = request.WebsiteUrl;
            }
        }
    }
}
