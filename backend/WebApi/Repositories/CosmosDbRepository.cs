using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Azure.Cosmos;
using WebApi.Models;

namespace WebApi.Repositories
{
    public abstract class CosmosDbRepository<TEntity, TDocument>
        where TEntity: Entity
        where TDocument : Document
    {
        protected readonly Container container;

        protected CosmosDbRepository(Container container)
        {
            ArgumentNullException.ThrowIfNull(container);
            this.container = container;
        }

        public virtual async Task<TEntity?> GetByIdAsync(string id, string partitionKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentException.ThrowIfNullOrWhiteSpace(partitionKey);

            try
            {
                var item = await this.container.ReadItemAsync<TDocument>(id, new PartitionKey(partitionKey), requestOptions: null, cancellationToken: cancellationToken);
                return this.ToEntity(item.Resource);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = "SELECT * FROM c";
            var queryDefinition = new QueryDefinition(query);

            using FeedIterator<TDocument> feedIterator = this.container.GetItemQueryIterator<TDocument>(queryDefinition);

            var results = new List<TDocument>();

            while (feedIterator.HasMoreResults)
            {
                FeedResponse<TDocument> response = await feedIterator.ReadNextAsync(cancellationToken);
                results.AddRange(response.Resource);
            }

            return results.Select(this.ToEntity);
        }

        public virtual async Task<DatabaseOperationStatus> UpsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(entity);
            cancellationToken.ThrowIfCancellationRequested();

            var document = this.ToDocument(entity);
            await this.container.UpsertItemAsync(document, new PartitionKey(document.PartitionKey), cancellationToken: cancellationToken);

            return DatabaseOperationStatus.Success;
        }

        public virtual async Task<DatabaseOperationStatus> DeleteAsync(string id, string partitionKey, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentException.ThrowIfNullOrWhiteSpace(partitionKey);

            try
            {
                await this.container.DeleteItemAsync<TDocument>(id, new PartitionKey(partitionKey));
                return DatabaseOperationStatus.Success;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return DatabaseOperationStatus.NotFound;
            }
        }

        protected abstract TEntity ToEntity(TDocument document);

        protected abstract TDocument ToDocument(TEntity entity);
    }
}
