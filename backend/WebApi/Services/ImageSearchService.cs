using Meilisearch;
using WebApi.Models;
using WebApi.Repositories;

public sealed class ImageSearchService
{
    private readonly MeilisearchClient client;
    private readonly ImageRepository repository;
    private const string IndexName = "images";

    public ImageSearchService(MeilisearchClient client, ImageRepository repository)
    {
        this.client = client;
        this.repository = repository;
    }

    public async Task IndexAsync(Image record)
    {
        var index = client.Index(IndexName);
        var doc = IndexedImage.From(record);
        await index.AddDocumentsAsync([doc]);
    }

    public async Task RemoveIndexAsync(string imageId)
    {
        var index = client.Index(IndexName);
        await index.DeleteOneDocumentAsync(imageId);
    }

    public async Task<List<Image>> SearchAsync(
        string query,
        string[]? filterTags = null,
        int limit = 20,
        CancellationToken ct = default)
    {
        var index = client.Index(IndexName);

        var searchParams = new SearchQuery
        {
            Limit = limit,
            Filter = filterTags?.Length > 0
                ? $"tags IN [{string.Join(", ", filterTags.Select(t => $"\"{t}\""))}]"
                : null,
        };

        var result = await index.SearchAsync<IndexedImage>(query, searchParams);
        var ids = result.Hits.Select(h => h.Id).ToList();
        var records = await this.repository.GetByIdsAsync(ids, ct);

        return records.ToList();
    }
}
