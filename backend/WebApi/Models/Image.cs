namespace WebApi.Models
{
    public class Image
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Path { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
