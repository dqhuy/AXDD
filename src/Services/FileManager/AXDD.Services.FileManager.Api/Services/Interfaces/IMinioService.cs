namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for MinIO operations
/// </summary>
public interface IMinioService
{
    /// <summary>
    /// Uploads a file to MinIO
    /// </summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="objectKey">The object key (path)</param>
    /// <param name="stream">The file stream</param>
    /// <param name="contentType">The content type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UploadFileAsync(
        string bucketName,
        string objectKey,
        Stream stream,
        string contentType,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Downloads a file from MinIO
    /// </summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="objectKey">The object key (path)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The file stream</returns>
    Task<Stream> DownloadFileAsync(
        string bucketName,
        string objectKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a file from MinIO
    /// </summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="objectKey">The object key (path)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteFileAsync(
        string bucketName,
        string objectKey,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a presigned URL for a file
    /// </summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="objectKey">The object key (path)</param>
    /// <param name="expirySeconds">URL expiry in seconds</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The presigned URL</returns>
    Task<string> GetPresignedUrlAsync(
        string bucketName,
        string objectKey,
        int expirySeconds,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a bucket exists
    /// </summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the bucket exists</returns>
    Task<bool> BucketExistsAsync(string bucketName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a bucket if it doesn't exist
    /// </summary>
    /// <param name="bucketName">The bucket name</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken = default);
}
