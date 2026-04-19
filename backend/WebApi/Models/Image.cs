namespace WebApi.Models
{
    public class Image : Entity
    {
        public string Path { get; set; }

        public string OriginalFileName { get; set; }

        public string ContentType { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public bool IsCensored { get; set; }

        public UploadStatus Status { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public long Size { get; set; }

        public bool IsFavorite { get; set; }

        public bool IsSoftDeleted { get; set; }

        public string ThumbnailPath { get; set; } = string.Empty;

        public string MediumPath { get; set; } = string.Empty;

        public string? Category { get; set; }

        public List<string> Tags { get; set; } = [];

        public string? CameraMake { get; set; }

        public string? CameraModel { get; set; }

        public string? TakenAt { get; set; }

        public ushort? Orientation { get; set; }

        public string? OriginalUrl { get; set; }
    }
}
