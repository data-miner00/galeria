using Provisioning;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using Spectre.Console;
using Meilisearch;

#if DEBUG
Console.WriteLine("Provisioning in debug mode.");
#else
Console.WriteLine("Provisioning in production mode.");
#endif

#if NET8_0 || NET10_0
Console.WriteLine("Provisioning in .NET.");
#else
#error No TFM is Implemented!
throw new PlatformNotSupportedException();
#endif

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("settings.json", optional: false, reloadOnChange: false)
    .Build();

AnsiConsole.Write(new FigletText("Galeria").Color(Color.CadetBlue));

var choice = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("What would you like to provision?")
        .PageSize(10)
        .AddChoices([
            "Cosmos DB containers",
            "Blob storage containers",
            "Meilisearch indexes",
            "All",
            "Exit"
        ]));

try
{
    await (choice switch
    {
        "Cosmos DB containers" => ProvisionCosmosAsync(configuration),
        "Blob storage containers" => ProvisionBlobAsync(configuration),
        "Meilisearch indexes" => ProvisionMeilisearchAsync(configuration),
        "All" => ProvisionAllAsync(configuration),
        _ => Task.CompletedTask
    });
}
catch (Exception ex)
{
    AnsiConsole.MarkupLineInterpolated($"[red]Error:[/] {ex.Message}");
}

async Task ProvisionAllAsync(IConfiguration config)
{
    await ProvisionCosmosAsync(config);
    await ProvisionBlobAsync(config);
    await ProvisionMeilisearchAsync(config);
}

async Task ProvisionCosmosAsync(IConfiguration config)
{
    var cosmosDbOptions = config.GetSection("CosmosDb").Get<CosmosDbProvisionOptions>();
    if (cosmosDbOptions == null)
    {
        AnsiConsole.MarkupLine("[yellow]CosmosDb section missing in settings.json. Skipping Cosmos provisioning.[/]");
        return;
    }

    var cosmosConnectionString = config.GetConnectionString("CosmosDb");
    if (string.IsNullOrWhiteSpace(cosmosConnectionString))
    {
        AnsiConsole.MarkupLine("[yellow]CosmosDb connection string is missing. Skipping Cosmos provisioning.[/]");
        return;
    }

    await AnsiConsole.Status()
        .Start("Provisioning Cosmos DB...", async ctx =>
        {
            var cosmosClient = new CosmosClient(cosmosConnectionString, new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                LimitToEndpoint = true
            });

            var database = await cosmosClient.CreateDatabaseIfNotExistsAsync(cosmosDbOptions.DatabaseName);

            foreach (var container in cosmosDbOptions.Containers ?? new List<string>())
            {
                ctx.Status($"Creating container {container}...");
                await database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties
                {
                    Id = container,
                    PartitionKeyPath = "/partitionKey",
                });
            }

            AnsiConsole.MarkupLine("[green]Cosmos DB provisioning complete.[/]");
        });
}

async Task ProvisionBlobAsync(IConfiguration config)
{
    var blobStorageOptions = config.GetSection("BlobStorage").Get<BlobStorageProvisionOptions>();
    if (blobStorageOptions == null)
    {
        AnsiConsole.MarkupLine("[yellow]BlobStorage section missing in settings.json. Skipping blob provisioning.[/]");
        return;
    }

    var blobStorageConnectionString = config.GetConnectionString("BlobStorage");
    if (string.IsNullOrWhiteSpace(blobStorageConnectionString))
    {
        AnsiConsole.MarkupLine("[yellow]BlobStorage connection string is missing. Skipping blob provisioning.[/]");
        return;
    }

    var blobClient = new BlobServiceClient(blobStorageConnectionString);

    await AnsiConsole.Status()
        .Start("Provisioning blob containers...", async ctx =>
        {
            foreach (var container in blobStorageOptions.Containers ?? new List<string>())
            {
                ctx.Status($"Creating blob container {container}...");
                try
                {
                    await blobClient.CreateBlobContainerAsync(container);
                    AnsiConsole.MarkupLineInterpolated($"[green]Created container:[/] {container}");
                }
                catch (Azure.RequestFailedException ex) when (ex.Status == 409)
                {
                    AnsiConsole.MarkupLineInterpolated($"[yellow]Container already exists:[/] {container}");
                }
            }

            AnsiConsole.MarkupLine("[green]Blob storage provisioning complete.[/]");
        });
}

async Task ProvisionMeilisearchAsync(IConfiguration config)
{
    var meili = config.GetSection("Meilisearch").Get<MeilisearchProvisionOptions>();
    if (meili == null)
    {
        AnsiConsole.MarkupLine("[yellow]Meilisearch section missing in settings.json. Skipping Meilisearch provisioning.[/]");
        return;
    }

    if (string.IsNullOrWhiteSpace(meili.Host))
    {
        AnsiConsole.MarkupLine("[yellow]Meilisearch host is not configured. Skipping.[/]");
        return;
    }

    var client = new MeilisearchClient(meili.Host, meili.ApiKey);
    var index = client.Index(meili.IndexName);

    await AnsiConsole.Status()
        .Start("Provisioning Meilisearch indexes...", async ctx =>
        {
            ctx.Status($"Creating index {index}...");
            try
            {
                if (meili.SearchableAttributes.Count > 0)
                {
                    await index.UpdateSearchableAttributesAsync(meili.SearchableAttributes);
                    AnsiConsole.MarkupLineInterpolated($"[green]Updated searchable attributes:[/] {string.Join(',', meili.SearchableAttributes)}");
                }

                if (meili.RankingRules.Count > 0)
                {
                    await index.UpdateRankingRulesAsync(meili.RankingRules);
                    AnsiConsole.MarkupLineInterpolated($"[green]Updated ranking rules:[/] {string.Join(',', meili.RankingRules)}");
                }

                if (meili.FilterableAttributes.Count > 0)
                {
                    await index.UpdateFilterableAttributesAsync(meili.FilterableAttributes);
                    AnsiConsole.MarkupLineInterpolated($"[green]Updated filterable attributes:[/] {string.Join(',', meili.FilterableAttributes)}");
                }

                if (meili.SortableAttributes.Count > 0)
                {
                    await index.UpdateSortableAttributesAsync(meili.SortableAttributes);
                    AnsiConsole.MarkupLineInterpolated($"[green]Updated sortable attributes:[/] {string.Join(',', meili.SortableAttributes)}");
                }
            }
            catch (Exception e)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Error creating index {index}:[/] {e.Message}");
            }

            AnsiConsole.MarkupLine("[green]Meilisearch provisioning complete.[/]");
        });
}
