namespace WebApi.Models
{
    public class ImageDocument : Document
    {
        public const string PartitionKeyValue = "ImagePartition";

        public string Path { get; set; }

        public string OriginalFileName { get; set; }

        public string ContentType { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public UploadStatus Status { get; set; }

        public bool IsCensored { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public long Size { get; set; }

        public string ThumbnailPath { get; set; } = string.Empty;

        public string MediumPath { get; set; } = string.Empty;

        public bool IsFavorite { get; set; }

        public bool IsSoftDeleted { get; set; }

        public string? Category { get; set; }

        public List<string> Tags { get; set; } = [];

        public string? CameraMake { get; set; }

        public string? CameraModel { get; set; }

        public string? TakenAt { get; set; }

        public ushort? Orientation { get; set; }

        public Image ToEntity()
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Timestamp ?? 0);
            DateTime updatedAt = dateTimeOffset.UtcDateTime;

            return new Image
            {
                Path = Path,
                Id = Id,
                Title = Title,
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
                ThumbnailPath = ThumbnailPath,
                MediumPath = MediumPath,
                IsFavorite = IsFavorite,
                IsSoftDeleted = IsSoftDeleted,
                Category = Category,
                Tags = Tags,
                CameraMake = CameraMake,
                CameraModel = CameraModel,
                TakenAt = TakenAt,
                Orientation = Orientation,
            };
        }

        public static ImageDocument FromEntity(Image entity)
        {
            return new ImageDocument
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Title = entity.Title,
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
                ThumbnailPath = entity.ThumbnailPath,
                MediumPath = entity.MediumPath,
                IsFavorite = entity.IsFavorite,
                IsSoftDeleted = entity.IsSoftDeleted,
                Category = entity.Category,
                Tags = entity.Tags,
                CameraMake = entity.CameraMake,
                CameraModel = entity.CameraModel,
                TakenAt = entity.TakenAt,
                Orientation = entity.Orientation,
            };
        }
    }
}
