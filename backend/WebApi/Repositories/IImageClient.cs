namespace WebApi.Repositories;

public interface IImageClient
{
    Task DeleteAsync(string blobName, CancellationToken ct = default);

    Task<Stream> DownloadAsync(string blobName, CancellationToken ct = default);

    Task<bool> ExistsAsync(string blobName, CancellationToken ct = default);

    Uri GetPublicUri(string blobName);

    Task<string> UploadAsync(Stream imageStream, string fileName, string contentType, CancellationToken ct = default);

    Task<Stream> DownloadAllAsync(CancellationToken ct = default);

    Task<Stream> DownloadMultipleAsync(List<string> paths, CancellationToken ct = default);
}