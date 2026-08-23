using Microsoft.AspNetCore.Http;

namespace Core.Services
{
    public sealed record UploadImageRequest
    {
        public IFormFile File { get; set; }

        public string? Title { get; set; }

        public bool IsCensored { get; set; }

        public string? OriginalUrl { get; set; }

        public bool IsAutoCaption { get; set; }
    }
}
