namespace WebApi.Dtos
{
    public class UpdateImageRequest
    {
        public string? Description { get; set; }

        public bool? IsCensored { get; set; }

        public bool? IsFavorite { get; set; }

        public bool? IsSoftDeleted { get; set; }

        public string? Category { get; set; }

        public List<string>? Tags { get; set; }
    }
}
