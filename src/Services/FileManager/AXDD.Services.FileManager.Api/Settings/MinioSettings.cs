namespace AXDD.Services.FileManager.Api.Settings;

/// <summary>
/// MinIO configuration settings
/// </summary>
public class MinioSettings
{
    /// <summary>
    /// Gets or sets the MinIO endpoint URL
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MinIO access key
    /// </summary>
    public string AccessKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the MinIO secret key
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether to use SSL
    /// </summary>
    public bool UseSSL { get; set; }

    /// <summary>
    /// Gets or sets the bucket names
    /// </summary>
    public BucketNames BucketNames { get; set; } = new();
}

/// <summary>
/// MinIO bucket names
/// </summary>
public class BucketNames
{
    /// <summary>
    /// Gets or sets the documents bucket name
    /// </summary>
    public string Documents { get; set; } = "axdd-documents";

    /// <summary>
    /// Gets or sets the attachments bucket name
    /// </summary>
    public string Attachments { get; set; } = "axdd-attachments";

    /// <summary>
    /// Gets or sets the temporary files bucket name
    /// </summary>
    public string Temp { get; set; } = "axdd-temp";

    /// <summary>
    /// Gets or sets the archives bucket name
    /// </summary>
    public string Archives { get; set; } = "axdd-archives";
}
