using Meilisearch;
using WebApi.Models;
using WebApi.Repositories;

public sealed class ImageSearchService
{
    private readonly Meilisearch.Index index;
    private readonly ImageRepository repository;

    public ImageSearchService(Meilisearch.Index index, ImageRepository repository)
    {
        this.index = index;
        this.repository = repository;
    }

    public async Task IndexAsync(Image record)
    {
        var doc = IndexedImage.From(record);
        await index.AddDocumentsAsync([doc]);
    }

    public async Task RemoveIndexAsync(string imageId)
    {
        await index.DeleteOneDocumentAsync(imageId);
    }

    public async Task<List<Image>> SearchAsync(
        string query,
        string[]? filterTags = null,
        int limit = 20,
        CancellationToken ct = default)
    {
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
