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

        [HttpPost]
        [RequestSizeLimit(TwentyMegabytes)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] UploadImageRequest request)
        {
            await this.service.UploadImageAsync(request, this.CancellationToken);

            return this.Created();
        }
    }
}
