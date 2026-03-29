using Microsoft.Azure.Cosmos;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class BoardRepository : CosmosDbRepository<Board, BoardDocument>
    {
        public BoardRepository(Container container) : base(container)
        {
        }

        public async Task<DatabaseOperationStatus> AddImageToBoardAsync(
            string id,
            string partitionKey,
            string imageId,
            CancellationToken ct)
        {
            var patchOperations = new[]
            {
                PatchOperation.Add("/ImageIds/-", imageId) 
            };

            ItemResponse<dynamic> response = await this.container.PatchItemAsync<dynamic>(
                id,
                partitionKey: new PartitionKey(partitionKey),
                patchOperations: patchOperations,
                cancellationToken: ct
            );

            return DatabaseOperationStatus.Success;
        }

        protected override BoardDocument ToDocument(Board entity)
        {
            return BoardDocument.FromEntity(entity);
        }

        protected override Board ToEntity(BoardDocument document)
        {
            return document.ToEntity();
        }
    }
}
