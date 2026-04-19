namespace WebApi.Dtos
{
    public sealed record UploadImageRequest
    {
        public IFormFile File { get; set; }

        public string? Title { get; set; }

        public bool IsCensored { get; set; }

        public string? OriginalUrl { get; set; }
    }
}
