namespace WebApi.Models
{
    public class BoardDocument : Document
    {
        public const string PartitionKeyValue = "default";

        public string Title { get; set; }

        public string? Description { get; set; }

        public List<string> ImageIds { get; set; } = [];

        public bool IsDeletable { get; set; }

        public DateTime CreatedAt { get; set; }

        public Board ToEntity()
        {
            return new Board
            {
                Id = Id,
                Title = Title,
                Description = Description,
                ImageIds = ImageIds,
                IsDeletable = IsDeletable,
                CreatedAt = CreatedAt,
            };
        }

        public static BoardDocument FromEntity(Board entity)
        {
            return new BoardDocument
            {
                PartitionKey = PartitionKeyValue,
                Id = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                ImageIds = entity.ImageIds,
                IsDeletable = entity.IsDeletable,
                CreatedAt = entity.CreatedAt,
            };
        }
    }
}
