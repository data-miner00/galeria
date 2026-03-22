
using Azure.Storage.Blobs;
using Microsoft.Azure.Cosmos;
using WebApi.Options;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.ConfigureRepositories();
            builder.ConfigureClients();
            builder.Services.AddSingleton<ImageService>();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(opt =>
                {
                    opt.SwaggerEndpoint("/openapi/v1.json", "Galeria API V1"); // /swagger/index
                });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }

        private static WebApplicationBuilder ConfigureRepositories(this WebApplicationBuilder builder)
        {
            var cosmosConnectionString = builder.Configuration.GetConnectionString("CosmosDb")
                ?? throw new InvalidOperationException("Cannot find Cosmos connection string");

            var opt = new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
            };

            var cosmosClient = new CosmosClient(cosmosConnectionString, opt);

            var repositoryFactory = new RepositoryFactory(cosmosClient);

            builder.Services.AddSingleton(repositoryFactory.CreateImageRepository());

            return builder;
        }

        private static WebApplicationBuilder ConfigureClients(this WebApplicationBuilder builder)
        {
            var storageAccountConnectionString = builder.Configuration.GetConnectionString("StorageAccount")
                ?? throw new InvalidOperationException("Cannot find blob storage connection string");

            var options = builder.Configuration.GetSection("BlobStorage").Get<BlobStorageOptions>();

            var blobClient = new BlobServiceClient(storageAccountConnectionString);
            var container = blobClient.GetBlobContainerClient(options.ContainerName);
            var facadeClient = new BlobStorageImageClient(container);

            builder.Services.AddSingleton<IImageClient>(facadeClient);

            return builder;
        }
    }
}
