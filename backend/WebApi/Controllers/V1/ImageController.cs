using Microsoft.AspNetCore.Mvc;
using WebApi.Dtos;
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
                return this.BadRequest();
            }

            var images = await this.repository.GetByIdsAsync(request.ImageIds, this.CancellationToken);

            return this.Ok(images);
        }
    }
}
