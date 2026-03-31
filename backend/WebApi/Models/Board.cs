namespace WebApi.Models
{
    public class Board : Entity
    {
        public string Title { get; set; }

        public string? Description { get; set; }

        public List<string> ImageIds { get; set; } = [];

        public bool IsDeletable { get; set; }
    }
}
