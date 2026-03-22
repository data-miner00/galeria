using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;
using WebApi.Repositories;

/// <summary>
/// Abstraction over Azure Blob Storage for image file operations.
/// Register as a scoped or singleton service in DI.
/// </summary>
public sealed class BlobStorageImageClient : IImageClient
{
    private readonly BlobContainerClient container;

    public BlobStorageImageClient(BlobContainerClient container)
    {
        this.container = container;
    }

    /// <summary>
    /// Uploads an image stream to blob storage.
    /// Returns the blob name (use this as the stable identifier in your DB).
    /// </summary>
    public async Task<string> UploadAsync(
        Stream imageStream,
        string fileName,
        string contentType,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(imageStream);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var blobClient = this.container.GetBlobClient(fileName);

        var uploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            }
        };

        await blobClient.UploadAsync(imageStream, uploadOptions, ct);

        return fileName;
    }

    /// <summary>
    /// Downloads an image as a stream. Caller is responsible for disposing the stream.
    /// </summary>
    public async Task<Stream> DownloadAsync(string blobName, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);

        var blobClient = this.container.GetBlobClient(blobName);

        try
        {
            var response = await blobClient.DownloadStreamingAsync(cancellationToken: ct);
            return response.Value.Content;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            throw new FileNotFoundException($"Blob '{blobName}' was not found in container.", ex);
        }
    }

    /// <summary>
    /// Deletes a blob. Does NOT throw if the blob does not exist.
    /// </summary>
    public async Task DeleteAsync(string blobName, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);

        var blobClient = this.container.GetBlobClient(blobName);

        // DeleteIfExistsAsync avoids a 404 race condition
        await blobClient.DeleteIfExistsAsync(
            snapshotsOption: DeleteSnapshotsOption.IncludeSnapshots,
            cancellationToken: ct);
    }

    /// <summary>
    /// Checks whether a blob exists in the container.
    /// </summary>
    public async Task<bool> ExistsAsync(string blobName, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);

        var blobClient = this.container.GetBlobClient(blobName);
        var response = await blobClient.ExistsAsync(ct);
        return response.Value;
    }

    /// <summary>
    /// Returns the public URI of a blob.
    /// Assumes the container has public read access, or you're building a SAS URI elsewhere.
    /// </summary>
    public Uri GetPublicUri(string blobName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(blobName);

        var blobClient = this.container.GetBlobClient(blobName);
        return blobClient.Uri;
    }
}
