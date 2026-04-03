namespace WebApi.Models
{
    public class ImageDocument : Document
    {
        public const string PartitionKeyValue = "ImagePartition";

        public string Path { get; set; }

        public string OriginalFileName { get; set; }

        public string ContentType { get; set; }

        public string? Description { get; set; }

        public UploadStatus Status { get; set; }

        public bool IsCensored { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public long Size { get; set; }

        public Image ToEntity()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Timestamp ?? 0);
            DateTime updatedAt = dateTimeOffset.UtcDateTime;

            return new Image
            {
                Path = Path,
                Id = Id,
                Description = Description,
                CreatedAt = CreatedAt,
                ETag = ETag,
                OriginalFileName = OriginalFileName,
                ContentType = ContentType,
                Status = Status,
                UpdatedAt = updatedAt,
                IsCensored = IsCensored,
                Width = Width,
                Height = Height,
                Size = Size,
            };
        }

        public static ImageDocument FromEntity(Image entity)
        {
            return new ImageDocument
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Description = entity.Description,
                ETag = entity.ETag,
                Path = entity.Path,
                PartitionKey = PartitionKeyValue,
                OriginalFileName = entity.OriginalFileName,
                ContentType = entity.ContentType,
                Status = entity.Status,
                DocumentType = DocumentType.ImageRecord,
                IsCensored = entity.IsCensored,
                Width = entity.Width,
                Height = entity.Height,
                Size = entity.Size,
            };
        }
    }
}
