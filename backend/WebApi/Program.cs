
using Azure.Storage.Blobs;
using Google.GenAI;
using Meilisearch;
using Microsoft.Azure.Cosmos;
using WebApi.Options;
using WebApi.Repositories;
using WebApi.Services;

namespace WebApi
{
    public static class Program
    {
        private static readonly string CorsPolicyName = "GaleriaPolicy";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.ConfigureRepositories()
                .ConfigureClients()
                .ConfigureCors()
                .ConfigureMeilisearch()
                .ConfigureGeminiClient();

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

            app.UseCors(CorsPolicyName);

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
            builder.Services.AddSingleton(repositoryFactory.CreateBoardRepository());
            builder.Services.AddSingleton(repositoryFactory.CreateUserProfileRepository());
            builder.Services.AddSingleton(repositoryFactory.CreateUserSettingsRepository());
            builder.Services.AddSingleton(repositoryFactory.CreateSecuritySettingsRepository());

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

        private static WebApplicationBuilder ConfigureCors(this WebApplicationBuilder builder)
        {
            var corsOptions = builder.Configuration.GetSection("Cors").Get<CorsOptions>()
                ?? throw new InvalidOperationException("Cors option not found.");

            builder.Services.AddCors((opt) =>
            {
                opt.AddPolicy(
                    name: CorsPolicyName,
                    (policy) =>
                    {
                        policy.WithOrigins(corsOptions.AllowedOrigins)
                              .WithHeaders(corsOptions.AllowedHeaders)
                              .WithMethods(corsOptions.AllowedMethods);
                    });
            });

            return builder;
        }

        private static WebApplicationBuilder ConfigureMeilisearch(this WebApplicationBuilder builder)
        {
            var options = builder.Configuration.GetSection("Meilisearch").Get<MeilisearchOptions>()
                ?? throw new InvalidOperationException("Meilisearch option not found.");

            var client = new MeilisearchClient(options.Host, options.ApiKey);
            var index = client.Index(options.IndexName);

            builder.Services.AddSingleton(index);

            return builder;
        }

        private static WebApplicationBuilder ConfigureGeminiClient(this WebApplicationBuilder builder)
        {
            var options = builder.Configuration.GetSection("Gemini").Get<GeminiOptions>()
                ?? throw new InvalidOperationException("Gemini option not found.");

            var client = new Client(apiKey: options.ApiKey);

            builder.Services.AddSingleton(client);
            builder.Services.AddSingleton(options);

            return builder;
        }
    }
}
