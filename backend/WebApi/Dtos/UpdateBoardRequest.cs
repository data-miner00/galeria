namespace WebApi.Dtos
{
    public class UpdateBoardRequest
    {
        public string? Title { get; set; }

        public string? Description { get; set; }

        public bool? IsPinned { get; set; }
    }
}
