using Provisioning;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Azure.Storage.Blobs;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("settings.json", optional: false, reloadOnChange: false)
    .Build();

var cosmosDbOptions = configuration.GetSection("CosmosDb").Get<CosmosDbProvisionOptions>()
    ?? throw new InvalidOperationException("Provisioning cannot be null");

var cosmosConnectionString = configuration.GetConnectionString("CosmosDb");
var cosmosClient = new CosmosClient(cosmosConnectionString, new CosmosClientOptions
{
    ConnectionMode = ConnectionMode.Gateway,
    LimitToEndpoint = true
});

var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDbOptions.DatabaseName);

foreach (var container in cosmosDbOptions.Containers)
{
    await database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties
    {
        Id = container,
        PartitionKeyPath = "/partitionKey",
    });
}

var blobStorageOptions = configuration.GetSection("BlobStorage").Get<BlobStorageProvisionOptions>()
    ?? throw new InvalidOperationException("Provisioning cannot be null");

var blobStorageConnectionString = configuration.GetConnectionString("BlobStorage");
var blobClient = new BlobServiceClient(blobStorageConnectionString);

foreach (var container in blobStorageOptions.Containers)
{
    try
    {
        await blobClient.CreateBlobContainerAsync(container);
    }
    catch (AggregateException ex)
    {
        Console.Error.WriteLine("The container already exist.");
    }
}

