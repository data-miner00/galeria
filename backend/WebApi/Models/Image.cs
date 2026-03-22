namespace WebApi.Models
{
    public class Image : Entity
    {
        public string Path { get; set; }

        public string OriginalFileName { get; set; }

        public string ContentType { get; set; }

        public string? Description { get; set; }

        public UploadStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
