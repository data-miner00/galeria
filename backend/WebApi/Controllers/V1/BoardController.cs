using Microsoft.AspNetCore.Mvc;
using WebApi.Repositories;
using WebApi.Dtos;
using BoardDocument = WebApi.Models.BoardDocument;
using InternalBoard = WebApi.Models.Board;

namespace WebApi.Controllers.V1
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BoardController : ControllerBase
    {
        private readonly BoardRepository repository;

        public BoardController(BoardRepository repository)
        {
            this.repository = repository;
        }

        private CancellationToken CancellationToken => this.HttpContext.RequestAborted;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Board>>> Get()
        {
            var boards = await this.repository.GetAllAsync(this.CancellationToken);

            return Ok(boards.Select(Board.FromInternal));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Board>> GetById(string id)
        {
            var board = await this.repository.GetByIdAsync(id, BoardDocument.PartitionKeyValue, this.CancellationToken);

            if (board is null)
            {
                return this.NotFound();
            }

            return this.Ok(Board.FromInternal(board));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBoardRequest request)
        {
            var id = Guid.NewGuid().ToString();

            var board = new InternalBoard
            {
                Id = id,
                Title = request.Title,
                Description = request.Description,
                ImageIds = [],
                IsDeletable = true,
                CreatedAt = DateTime.UtcNow,
            };

            var result = await this.repository.UpsertAsync(board, this.CancellationToken);

            if (result != Models.DatabaseOperationStatus.Success)
            {
                return this.StatusCode(500, new ErrorResponse
                {
                    ErrorMessage = "The database did not indicate success.",
                    ReferenceId = Guid.NewGuid().ToString(),
                });
            }

            return this.Created(id, board);
        }

        [HttpPost("{boardId}/{imageId}")]
        public async Task<IActionResult> AddImage(string boardId, string imageId)
        {
            var status = await this.repository.AddImageToBoardAsync(boardId, BoardDocument.PartitionKeyValue, imageId, this.CancellationToken);

            if (status != Models.DatabaseOperationStatus.Success)
            {
                return this.NotFound();
            }

            return this.Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var status = await this.repository.DeleteAsync(id, BoardDocument.PartitionKeyValue, this.CancellationToken);

            if (status != Models.DatabaseOperationStatus.Success)
            {
                return this.NotFound();
            }

            return this.NoContent();
        }
    }
}
