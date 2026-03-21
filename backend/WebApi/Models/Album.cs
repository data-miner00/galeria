namespace WebApi.Models
{
    public class Album : Entity
    {
        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
