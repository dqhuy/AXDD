using AXDD.Services.FileManager.Api.Services.Interfaces;
using AXDD.Services.FileManager.Api.Settings;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for MinIO operations
/// </summary>
public class MinioService : IMinioService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioSettings _settings;
    private readonly ILogger<MinioService> _logger;

    /// <summary>
    /// Initializes a new instance of MinioService
    /// </summary>
    public MinioService(
        IMinioClient minioClient,
        IOptions<MinioSettings> settings,
        ILogger<MinioService> logger)
    {
        _minioClient = minioClient;
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task UploadFileAsync(
        string bucketName,
        string objectKey,
        Stream stream,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bucketName, nameof(bucketName));
        ArgumentException.ThrowIfNullOrWhiteSpace(objectKey, nameof(objectKey));
        ArgumentNullException.ThrowIfNull(stream, nameof(stream));

        try
        {
            _logger.LogInformation("Uploading file to MinIO: Bucket={Bucket}, Key={Key}", bucketName, objectKey);

            await EnsureBucketExistsAsync(bucketName, cancellationToken);

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectKey)
                .WithStreamData(stream)
                .WithObjectSize(stream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            _logger.LogInformation("File uploaded successfully to MinIO: Bucket={Bucket}, Key={Key}", bucketName, objectKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file to MinIO: Bucket={Bucket}, Key={Key}", bucketName, objectKey);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<Stream> DownloadFileAsync(
        string bucketName,
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bucketName, nameof(bucketName));
        ArgumentException.ThrowIfNullOrWhiteSpace(objectKey, nameof(objectKey));

        try
        {
            _logger.LogInformation("Downloading file from MinIO: Bucket={Bucket}, Key={Key}", bucketName, objectKey);

            var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectKey)
                .WithCallbackStream(async (stream, ct) =>
                {
                    await stream.CopyToAsync(memoryStream, ct);
                });

            await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);

            memoryStream.Position = 0;

            _logger.LogInformation("File downloaded successfully from MinIO: Bucket={Bucket}, Key={Key}", bucketName, objectKey);

            return memoryStream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading file from MinIO: Bucket={Bucket}, Key={Key}", bucketName, objectKey);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteFileAsync(
        string bucketName,
        string objectKey,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bucketName, nameof(bucketName));
        ArgumentException.ThrowIfNullOrWhiteSpace(objectKey, nameof(objectKey));

        try
        {
            _logger.LogInformation("Deleting file from MinIO: Bucket={Bucket}, Key={Key}", bucketName, objectKey);

            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectKey);

            await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);

            _logger.LogInformation("File deleted successfully from MinIO: Bucket={Bucket}, Key={Key}", bucketName, objectKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file from MinIO: Bucket={Bucket}, Key={Key}", bucketName, objectKey);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<string> GetPresignedUrlAsync(
        string bucketName,
        string objectKey,
        int expirySeconds,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bucketName, nameof(bucketName));
        ArgumentException.ThrowIfNullOrWhiteSpace(objectKey, nameof(objectKey));

        try
        {
            _logger.LogInformation("Generating presigned URL for MinIO object: Bucket={Bucket}, Key={Key}", bucketName, objectKey);

            var presignedGetObjectArgs = new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectKey)
                .WithExpiry(expirySeconds);

            var url = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);

            _logger.LogInformation("Presigned URL generated successfully: Bucket={Bucket}, Key={Key}", bucketName, objectKey);

            return url;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL: Bucket={Bucket}, Key={Key}", bucketName, objectKey);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> BucketExistsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bucketName, nameof(bucketName));

        try
        {
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(bucketName);

            return await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if bucket exists: Bucket={Bucket}", bucketName);
            throw;
        }
    }

    /// <inheritdoc/>
    public async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bucketName, nameof(bucketName));

        try
        {
            var exists = await BucketExistsAsync(bucketName, cancellationToken);

            if (!exists)
            {
                _logger.LogInformation("Creating MinIO bucket: {Bucket}", bucketName);

                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(bucketName);

                await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);

                _logger.LogInformation("MinIO bucket created successfully: {Bucket}", bucketName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ensuring bucket exists: Bucket={Bucket}", bucketName);
            throw;
        }
    }
}
