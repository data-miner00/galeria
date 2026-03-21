using Provisioning;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("settings.json", optional: false, reloadOnChange: false)
    .Build();

var provisioning = configuration.GetSection("Provisioning").Get<Options>()
    ?? throw new InvalidOperationException("Provisioning cannot be null");

var connectionString = configuration.GetConnectionString("CosmosDb");
var cosmosClient = new CosmosClient(connectionString, new CosmosClientOptions
{
    ConnectionMode = ConnectionMode.Gateway,
    LimitToEndpoint = true
});

var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(provisioning.DatabaseName);

foreach (var container in provisioning.Containers)
{
    await database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties
    {
        Id = container,
        PartitionKeyPath = "/partitionKey",
    });
}
