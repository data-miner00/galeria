namespace WebApi.Models
{
    public class Image : Entity
    {
        public string Path { get; set; }

        public string OriginalFileName { get; set; }

        public string ContentType { get; set; }

        public string? Description { get; set; }

        public bool IsCensored { get; set; }

        public UploadStatus Status { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public long Size { get; set; }

        public string ThumbnailPath { get; set; } = string.Empty;

        public string MediumPath { get; set; } = string.Empty;
    }
}
