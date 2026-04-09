using InternalBoard = WebApi.Models.Board;

namespace WebApi.Dtos
{
    public class Board
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public List<string> ImageIds { get; set; } = [];

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public bool IsPinned { get; set; }

        public static Board FromInternal(InternalBoard board)
        {
            return new Board
            {
                Id = board.Id,
                Title = board.Title,
                Description = board.Description,
                ImageIds = board.ImageIds,
                CreatedAt = board.CreatedAt,
                UpdatedAt = board.UpdatedAt,
                IsPinned = board.IsPinned,
            };
        }
    }
}
