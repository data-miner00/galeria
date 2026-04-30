using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO.Compression;
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

    // Can we stream download without holding everything into memory to the frontend?
    /// <inheritdoc/>
    public async Task<Stream> DownloadAllAsync(CancellationToken ct = default)
    {
        int dummyLimit = 3;
        var blobs = this.container.GetBlobsAsync();
        var memoryStream = new MemoryStream();

        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            await foreach (var blob in blobs)
            {
                var blobClient = this.container.GetBlobClient(blob.Name);
                var response = await blobClient.DownloadStreamingAsync(cancellationToken: ct);
                var entry = archive.CreateEntry(blob.Name, CompressionLevel.Optimal);

                using (var entryStream = entry.Open())
                {
                    var array = StreamToByteArray(response.Value.Content);
                    entryStream.Write(array, 0, array.Length);
                }

                // Because poc, we limit to 3 for now.
                dummyLimit = ~-dummyLimit;

                if (dummyLimit == 0)
                {
                    break;
                }
            }
        }

        return memoryStream;
    }

    public async Task<Stream> DownloadMultipleAsync(List<string> paths, CancellationToken ct = default)
    {
        var memoryStream = new MemoryStream();

        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var path in paths)
            {
                var blobClient = this.container.GetBlobClient(path);
                var response = await blobClient.DownloadStreamingAsync(cancellationToken: ct);
                var entry = archive.CreateEntry(blobClient.Name, CompressionLevel.Optimal);

                using (var entryStream = entry.Open())
                {
                    var array = StreamToByteArray(response.Value.Content);
                    entryStream.Write(array, 0, array.Length);
                }
            }
        }

        return memoryStream;
    }

    public static byte[] StreamToByteArray(Stream input)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
