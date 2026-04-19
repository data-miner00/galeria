using WebApi.Dtos;
using WebApi.Models;
using WebApi.Repositories;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixImage = SixLabors.ImageSharp.Image;
using ImageRecord = WebApi.Models.Image;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using Google.GenAI;
using WebApi.Clients;

namespace WebApi.Services;

public sealed class ImageService
{
    private readonly ImageRepository repository;
    private readonly IImageClient blobClient;
    private readonly Meilisearch.Index index;
    private readonly IGenAiClient genai;
    private readonly ILogger<ImageService> logger;
 
    public ImageService(
        ImageRepository repository,
        IImageClient blobClient,
        Meilisearch.Index index,
        IGenAiClient genai,
        ILogger<ImageService> logger)
    {
        this.repository = repository;
        this.blobClient = blobClient;
        this.index = index;
        this.genai = genai;
        this.logger = logger;
    }
 
    /// <summary>
    /// Creates a DB record first, then uploads to Blob Storage.
    /// If the blob upload fails, the DB record is marked as Failed
    /// so it can be retried or cleaned up — no silent ghost records.
    /// </summary>
    public async Task<ImageRecord> UploadImageAsync(
        UploadImageRequest request,
        CancellationToken ct = default)
    {
        var file = request.File;

        // 1. Pre-compute deterministic blob paths so clients can construct
        //    variant URLs from the image id without reading the DB.
        var extension = Path.GetExtension(file.FileName);
        var id = Guid.NewGuid().ToString();
        var basePath = id;
        var originalPath = $"{basePath}/original{extension}";
        var thumbPath = $"{basePath}/thumb/150{extension}";
        var mediumPath = $"{basePath}/medium/w1024{extension}";

        // Read the incoming file into memory so we can upload multiple variants
        // and inspect metadata without re-reading the IFormFile stream.
        await using var uploadedStream = file.OpenReadStream();
        using var buffered = new MemoryStream();
        await uploadedStream.CopyToAsync(buffered, ct);
        buffered.Position = 0;

        var record = new ImageRecord
        {
            Id = id,
            OriginalFileName = file.FileName,
            Path = originalPath,
            ContentType = file.ContentType,
            Title = request.Title,
            Status = UploadStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            IsCensored = request.IsCensored,
            Size = buffered.Length,
            ThumbnailPath = thumbPath,
            MediumPath = mediumPath,
        };

        if (!string.IsNullOrWhiteSpace(request.OriginalUrl))
        {
            record.OriginalUrl = request.OriginalUrl;
        }

        PopulateMetadata(buffered, record);

        var captionResponse = await this.genai.GenerateCaptionAsync(buffered, MimeTypes.GetMimeType(file.FileName));

        if (captionResponse is not null)
        {
            record.Description = captionResponse.Description;
            record.Tags = captionResponse.Tags;
        }

        await this.repository.UpsertAsync(record, ct);

        this.logger.LogInformation("Image record {Id} created with status Pending.", record.Id);

        try
        {
            // Upload original
            buffered.Position = 0;
            await this.blobClient.UploadAsync(buffered, originalPath, file.ContentType, ct);

            // Load image for resizing
            buffered.Position = 0;
            using var image = SixImage.Load(buffered);

            // Encoder selection based on extension — preserve original format where possible
            IImageEncoder encoder = extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => new JpegEncoder { Quality = 80 },
                ".png" => new PngEncoder { CompressionLevel = PngCompressionLevel.Level6 },
                _ => new JpegEncoder { Quality = 80 },
            };

            // Medium: max width 1024, preserve aspect ratio, never upscale
            if (image.Width > 1024)
            {
                using var medium = image.Clone(ctx => ctx.Resize(new ResizeOptions
                {
                    Size = new Size(1024, 0),
                    Mode = ResizeMode.Max
                }));

                await using var msMedium = new MemoryStream();
                await medium.SaveAsync(msMedium, encoder, ct);
                msMedium.Position = 0;
                await this.blobClient.UploadAsync(msMedium, mediumPath, file.ContentType, ct);
            }
            else
            {
                // If original is smaller than medium target, reuse original blob by uploading a copy
                buffered.Position = 0;
                await this.blobClient.UploadAsync(buffered, mediumPath, file.ContentType, ct);
            }

            // Thumbnail: square crop center 150x150
            using var thumb = image.Clone(ctx => ctx.Resize(new ResizeOptions
            {
                Size = new Size(150, 150),
                Mode = ResizeMode.Crop,
                Position = AnchorPositionMode.Center
            }));

            await using var msThumb = new MemoryStream();
            await thumb.SaveAsync(msThumb, encoder, ct);
            msThumb.Position = 0;
            await this.blobClient.UploadAsync(msThumb, thumbPath, file.ContentType, ct);

            record.Status = UploadStatus.Suceeded;
            await this.repository.UpsertAsync(record, ct);
            await this.index.AddDocumentsAsync([record]);

            this.logger.LogInformation("Image record {Id} uploaded successfully with variants.", record.Id);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Blob upload failed for image record {Id}.", record.Id);

            record.Status = UploadStatus.Failed;
            await this.repository.UpsertAsync(record, ct);
        }

        return record;
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
        ArgumentException.ThrowIfNullOrWhiteSpace(imageId);
        ct.ThrowIfCancellationRequested();

        var record = await this.repository.GetByIdAsync(imageId, ImageDocument.PartitionKeyValue, ct);

        if (record is null)
        {
            return false;
        }

        await this.repository.DeleteAsync(imageId, ImageDocument.PartitionKeyValue, ct);

        List<Task> tasks = [
            this.blobClient.DeleteAsync(record.Path, ct),
            this.blobClient.DeleteAsync(record.ThumbnailPath, ct),
            this.blobClient.DeleteAsync(record.MediumPath, ct),
            this.index.DeleteOneDocumentAsync(imageId),
        ];

        await Task.WhenAll(tasks);

        return true;
    }

    /// <summary>
    /// Permanently deletes all images that have been soft deleted from the repository.
    /// </summary>
    /// <param name="ct">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>true if all soft deleted images are successfully deleted; otherwise, false.</returns>
    public async Task<bool> DeleteSoftDeletedImagesAsync(CancellationToken ct = default)
    {
        var recycledImages = await this.repository.GetAllSoftDeletedAsync(ct);

        List<Task<DatabaseOperationStatus>> deleteTasks = recycledImages
            .Select(image => this.repository.DeleteAsync(image.Id, ImageDocument.PartitionKeyValue, ct))
            .ToList();

        await Task.WhenAll(deleteTasks);

        return true;
    }

    public async Task<List<ImageRecord>> SearchAsync(
        string query,
        string[]? filterTags = null,
        int limit = 20,
        CancellationToken ct = default)
    {
        var searchParams = new Meilisearch.SearchQuery
        {
            Limit = limit,
            Filter = filterTags?.Length > 0
                ? $"tags IN [{string.Join(", ", filterTags.Select(t => $"\"{t}\""))}]"
                : null,
        };

        var result = await this.index.SearchAsync<IndexedImage>(query, searchParams);
        var ids = result.Hits.Select(h => h.Id).ToList();
        var records = await this.repository.GetByIdsAsync(ids, ct);

        return records.ToList();
    }

    private static void PopulateMetadata(MemoryStream buffered, ImageRecord record)
    {
        var metadata = SixImage.Identify(buffered);
        buffered.Position = 0;

        record.Width = metadata.Width;
        record.Height = metadata.Height;

        if (metadata.Metadata.ExifProfile is { } exif)
        {
            if (exif.TryGetValue(ExifTag.Make, out var exifMake))
            {
                record.CameraMake = exifMake.Value;
            }

            if (exif.TryGetValue(ExifTag.Model, out var exifModel))
            {
                record.CameraModel = exifModel.Value;
            }

            if (exif.TryGetValue(ExifTag.DateTimeOriginal, out var exifOriginal))
            {
                record.TakenAt = exifOriginal.Value;
            }

            if (exif.TryGetValue(ExifTag.Orientation, out var exifOrientation))
            {
                record.Orientation = exifOrientation.Value;
            }
        }
    }
}
