namespace WebApi.Models
{
    public class IndexedImage
    {
        public string Id { get; set; }

        public string OriginalFileName { get; set; }

        public string ContentType { get; set; }

        public string? Description { get; set; }

        public string? Category { get; set; }

        public List<string> Tags { get; set; } = [];

        public static IndexedImage From(Image image)
        {
            return new IndexedImage
            {
                Id = image.Id,
                OriginalFileName = image.OriginalFileName,
                ContentType = image.ContentType,
                Description = image.Description,
                Category = image.Category,
                Tags = image.Tags,
            };
        }
    }
}
