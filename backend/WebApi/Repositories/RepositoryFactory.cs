using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Repositories
{
    public sealed class RepositoryFactory
    {
        private const string DatabaseName = "Galeria";
        private readonly CosmosClient cosmosClient;

        public RepositoryFactory(CosmosClient cosmosClient)
        {
            this.cosmosClient = cosmosClient;
        }

        public ImageRepository CreateImageRepository()
        {
            const string ContainerName = "Image";
            var container = this.cosmosClient.GetContainer(DatabaseName, ContainerName);
            return new ImageRepository(container);
        }

        public BoardRepository CreateBoardRepository()
        {
            const string ContainerName = "Board";
            var container = this.cosmosClient.GetContainer(DatabaseName, ContainerName);
            return new BoardRepository(container);
        }
    }
}
