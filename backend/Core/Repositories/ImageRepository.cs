using Microsoft.Azure.Cosmos;
using Core.Models;

namespace Core.Repositories
{
    public class ImageRepository : CosmosDbRepository<Image, ImageDocument>
    {
        public ImageRepository(Container container) : base(container)
        {
        }

        public Task<IEnumerable<Image>> GetByIdsAsync(List<string> ids, CancellationToken ct)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE ARRAY_CONTAINS(@imageIds, c.id)")
                .WithParameter("@imageIds", ids.ToArray());

            return this.QueryAllAsync(query, ct);
        }

        public Task SoftDeleteByIdsAsync(List<string> ids, CancellationToken ct)
        {
            var deletedAt = DateTime.UtcNow;

            var tasks = ids.Select(
                id => this.container.PatchItemAsync<ImageDocument>(
                    id,
                    new PartitionKey(ImageDocument.PartitionKeyValue),
                    [PatchOperation.Replace("/IsSoftDeleted", true), PatchOperation.Set("/DeletedAt", deletedAt)],
                    cancellationToken: ct));

            return Task.WhenAll(tasks);
        }

        /// <summary>
        /// Gets all images that is not soft deleted.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The list of non-soft deleted images.</returns>
        public Task<IEnumerable<Image>> GetAllWithoutSoftDeletedAsync(CancellationToken ct)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.IsSoftDeleted = false OR NOT IS_DEFINED(c.IsSoftDeleted)");
            return this.QueryAllAsync(query, ct);
        }

        public Task<IEnumerable<Image>> GetAllFavoritesAsync(CancellationToken ct)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.IsFavorite = true");
            return this.QueryAllAsync(query, ct);
        }
        
        public Task<IEnumerable<Image>> GetAllSoftDeletedAsync(CancellationToken ct)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE c.IsSoftDeleted = true");
            return this.QueryAllAsync(query, ct);
        }

        /// <summary>
        /// Gets the ids of every document in the container, including soft deleted images.
        /// </summary>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>The set of document ids.</returns>
        public async Task<HashSet<string>> GetAllIdsAsync(CancellationToken ct)
        {
            var query = new QueryDefinition("SELECT VALUE c.id FROM c");

            using FeedIterator<string> feed = this.container.GetItemQueryIterator<string>(query);

            HashSet<string> ids = [];
            while (feed.HasMoreResults)
            {
                foreach (string id in await feed.ReadNextAsync(ct))
                {
                    ids.Add(id);
                }
            }

            return ids;
        }

        public async Task<IEnumerable<string>> GetUniqueCategories(CancellationToken ct)
        {
            var query = new QueryDefinition("SELECT DISTINCT VALUE c.Category FROM c");

            using FeedIterator<string> feed = container.GetItemQueryIterator<string>(query);

            List<string> result = new List<string>();
            while (feed.HasMoreResults)
            {
                foreach (string name in await feed.ReadNextAsync())
                {
                    result.Add(name);
                }
            }

            return result;
        }

        protected override DocumentType DocumentType => DocumentType.ImageRecord;

        protected override ImageDocument ToDocument(Image entity)
        {
            return ImageDocument.FromEntity(entity);
        }

        protected override Image ToEntity(ImageDocument document)
        {
            return document.ToEntity();
        }

        private async Task<IEnumerable<Image>> QueryAllAsync(QueryDefinition query, CancellationToken ct)
        {
            List<ImageDocument> images = [];

            var queryIterator = this.container.GetItemQueryIterator<ImageDocument>(query);

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync(ct);
                images.AddRange(response.Resource);
            }

            return images.Select(x => x.ToEntity());
        }
    }
}
