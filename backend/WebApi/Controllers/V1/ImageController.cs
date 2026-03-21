using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly ImageRepository repository;

        public ImageController(ImageRepository repository)
        {
            this.repository = repository;
        }

        private CancellationToken CancellationToken => this.HttpContext.RequestAborted;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Image image)
        {
            await this.repository.UpsertAsync(image, this.CancellationToken);

            return this.Created();
        }
    }
}
