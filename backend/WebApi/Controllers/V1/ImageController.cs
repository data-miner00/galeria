using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
using WebApi.Extensions;
using WebApi.Models;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ImageController : ControllerBase
    {
        private const int TwentyMegabytes = 20 * 1024 * 1024;

        private readonly ImageRepository repository;
        private readonly ImageService service;

        public ImageController(ImageService service, ImageRepository repository)
        {
            this.service = service;
            this.repository = repository;
        }

        private CancellationToken CancellationToken => this.HttpContext.RequestAborted;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Image>>> GetAll()
        {
            var images = await this.repository.GetAllAsync(this.CancellationToken);

            return this.Ok(images);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Image>> GetById(string id)
        {
            var image = await this.repository.GetByIdAsync(id, ImageDocument.PartitionKeyValue, this.CancellationToken);

            if (image is null)
            {
                return this.NotFound();
            }

            return this.Ok(image);
        }

        [HttpPost]
        [RequestSizeLimit(TwentyMegabytes)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] UploadImageRequest request)
        {
            if (!request.File.IsValidImageFile()) // TODO: Make this into an injectable service
            {
                return this.BadRequest(new ErrorResponse
                {
                    ErrorMessage = "File with unsupported format provided.",
                    ReferenceId = Guid.NewGuid().ToString(),
                });
            }

            var image = await this.service.UploadImageAsync(request, this.CancellationToken);

            return this.Created(image.Path, image);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var isDeleted = await this.service.DeleteAsync(id, this.CancellationToken);

            return isDeleted ? this.NoContent() : this.NotFound();
        }

        [HttpPost("getbyids")]
        public async Task<ActionResult<IEnumerable<Image>>> GetByIds([FromBody] GetImagesByIdsRequest request)
        {
            if (request is null || request.ImageIds.Count == 0)
            {
                return this.BadRequest(new ErrorResponse
                {
                    ErrorMessage = "The imageIds field must not be empty.",
                    ReferenceId = Guid.NewGuid().ToString(),
                });
            }

            var images = await this.repository.GetByIdsAsync(request.ImageIds, this.CancellationToken);

            return this.Ok(images);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Image>> UpdateImage([FromBody] UpdateImageRequest request, string id)
        {
            var image = await this.repository.GetByIdAsync(id, ImageDocument.PartitionKeyValue, this.CancellationToken);

            if (image == null)
            {
                return this.NotFound();
            }

            PatchImageFromRequest(image, request);

            await this.repository.UpsertAsync(image, this.CancellationToken);

            return this.Ok(image);
        }

        [HttpDelete("recyclebin/clear")]
        public async Task<IActionResult> ClearRecycleBin()
        {
            var isSuccess = await this.service.DeleteSoftDeletedImagesAsync(this.CancellationToken);

            return isSuccess ? this.NoContent() : this.StatusCode(500, new ErrorResponse
            {
                ReferenceId = Guid.NewGuid().ToString(),
                ErrorMessage = "Something went wrong during the clearing of the recycle bin.",
            });
        }

        private static void PatchImageFromRequest(Image image, UpdateImageRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Description))
            {
                image.Description = request.Description;
            }

            if (request.IsCensored.HasValue)
            {
                image.IsCensored = request.IsCensored.Value;
            }

            if (request.IsFavorite.HasValue)
            {
                image.IsFavorite = request.IsFavorite.Value;
            }

            if (request.IsSoftDeleted.HasValue)
            {
                image.IsSoftDeleted = request.IsSoftDeleted.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.Category))
            {
                image.Category = request.Category;
            }

            if (request.Tags is not null)
            {
                image.Tags = request.Tags;
            }
        }
    }
}
