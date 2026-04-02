namespace WebApi.Dtos
{
    public sealed record UploadImageRequest
    {
        public IFormFile File { get; set; }

        public string? Description { get; set; }

        public bool IsCensored { get; set; }
    }
}
