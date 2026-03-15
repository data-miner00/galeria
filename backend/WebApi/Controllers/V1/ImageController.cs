using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ImageController : ControllerBase
    {
        [HttpGet]
        public IEnumerable<Image> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new Image
            {
                Id = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Path = "/path",
                CreatedAt = DateTimeOffset.UtcNow,
            })
            .ToArray();
        }
    }
}
