namespace WebApi.Models
{
    public class Image : Entity
    {
        public string Path { get; set; }

        public string? Description { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
