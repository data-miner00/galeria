using Microsoft.Azure.Cosmos;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class ImageRepository : CosmosDbRepository<Image, ImageDocument>
    {
        public ImageRepository(Container container) : base(container)
        {
        }

        public async Task<IEnumerable<Image>> GetByIdsAsync(List<string> ids, CancellationToken ct)
        {
            var query = new QueryDefinition("SELECT * FROM c WHERE ARRAY_CONTAINS(@imageIds, c.id)")
                .WithParameter("@imageIds", ids.ToArray());

            List<ImageDocument> images = [];

            var queryIterator = this.container.GetItemQueryIterator<ImageDocument>(query);

            while (queryIterator.HasMoreResults)
            {
                var response = await queryIterator.ReadNextAsync(ct);
                images.AddRange(response.Resource);
            }

            return images.Select(x => x.ToEntity());
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
    }
}
