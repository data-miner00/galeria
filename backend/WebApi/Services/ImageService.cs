using WebApi.Dtos;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Services;

public sealed class ImageService
{
    private readonly ImageRepository repository;
    private readonly IImageClient blobClient;
    private readonly ILogger<ImageService> logger;
 
    public ImageService(
        ImageRepository repository,
        IImageClient blobClient,
        ILogger<ImageService> logger)
    {
        this.repository = repository;
        this.blobClient = blobClient;
        this.logger = logger;
    }
 
    /// <summary>
    /// Creates a DB record first, then uploads to Blob Storage.
    /// If the blob upload fails, the DB record is marked as Failed
    /// so it can be retried or cleaned up — no silent ghost records.
    /// </summary>
    public async Task<Image> UploadImageAsync(
        UploadImageRequest request,
        CancellationToken ct = default)
    {
        var file = request.File;
 
        // 1. Pre-compute the blob name here so it's stored in the DB
        //    before we attempt the upload — this is our reconciliation key.
        var extension = Path.GetExtension(file.FileName);
        var id = Guid.NewGuid().ToString();
        var blobName = $"{id}{extension}";

        var record = new Image
        {
            Id = id,
            OriginalFileName = file.FileName,
            Path = blobName,
            ContentType = file.ContentType,
            Description = request.Description,
            Status = UploadStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            IsCensored = request.IsCensored,
        };

        await this.repository.UpsertAsync(record, ct);
 
        this.logger.LogInformation("Image record {Id} created with status Pending.", record.Id);
 
        try
        {
            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, blobName, file.ContentType, ct);
 
            record.Status = UploadStatus.Suceeded;
            await this.repository.UpsertAsync(record, ct);
 
            this.logger.LogInformation("Image record {Id} uploaded successfully.", record.Id);
        }
        catch (Exception ex)
        {
            // 5. Mark as failed — do NOT delete the DB record.
            //    A retry job or admin can pick up Failed records.
            this.logger.LogError(ex, "Blob upload failed for image record {Id}.", record.Id);
 
            record.Status = UploadStatus.Failed;
            await this.repository.UpsertAsync(record, ct);
        }
 
        return record;
    }
 
    /// <summary>
    /// Retries uploading a previously Failed image.
    /// Call this from an admin endpoint or a background job.
    /// </summary>
    public async Task<bool> RetryFailedUploadAsync(string imageId, IFormFile file, CancellationToken ct = default)
    {
        var record = await this.repository.GetByIdAsync(imageId, ImageDocument.PartitionKeyValue, ct);
 
        if (record is null || record.Status == UploadStatus.Suceeded)
        {
            return false;
        }
 
        try
        {
            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, record.Path, record.ContentType, ct);
 
            record.Status = UploadStatus.Suceeded;
            await this.repository.UpsertAsync(record, ct);
            return true;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Retry upload failed for image record {Id}.", record.Id);
            return false;
        }
    }

    /// <summary>
    /// Deletes the image associated with the specified identifier asynchronously.
    /// </summary>
    /// <remarks>If the image record is found, this method deletes both the record from the repository and the
    /// associated blob from storage. If the record does not exist, no deletions are performed and the method returns
    /// <see langword="false"/>.</remarks>
    /// <param name="imageId">The unique identifier of the image to delete. Cannot be null or empty.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the operation. The default value is <see
    /// cref="CancellationToken.None"/>.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <see langword="true"/> if the image was
    /// successfully deleted; otherwise, <see langword="false"/> if no image was found with the specified identifier.</returns>
    public async Task<bool> DeleteAsync(string imageId, CancellationToken ct = default)
    {
        var record = await this.repository.GetByIdAsync(imageId, ImageDocument.PartitionKeyValue, ct);

        if (record is null)
        {
            return false;
        }

        await this.repository.DeleteAsync(imageId, ImageDocument.PartitionKeyValue, ct);
        await this.blobClient.DeleteAsync(record.Path, ct);

        return true;
    }
}
