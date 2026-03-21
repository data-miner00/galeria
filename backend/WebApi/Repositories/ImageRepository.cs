using Microsoft.Azure.Cosmos;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class ImageRepository : CosmosDbRepository<Image, ImageDocument>
    {
        public ImageRepository(Container container) : base(container)
        {
        }

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
