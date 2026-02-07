namespace AXDD.Services.FileManager.Api.Settings;

/// <summary>
/// File upload configuration settings
/// </summary>
public class FileUploadSettings
{
    /// <summary>
    /// Gets or sets the maximum file size in bytes
    /// </summary>
    public long MaxFileSizeBytes { get; set; } = 104857600; // 100MB

    /// <summary>
    /// Gets or sets the allowed file extensions
    /// </summary>
    public List<string> AllowedExtensions { get; set; } = new()
    {
        ".pdf", ".doc", ".docx", ".xls", ".xlsx",
        ".png", ".jpg", ".jpeg", ".gif", ".bmp",
        ".txt", ".csv", ".zip", ".rar"
    };

    /// <summary>
    /// Gets or sets whether to enable virus scanning (placeholder)
    /// </summary>
    public bool EnableVirusScanning { get; set; } = false;

    /// <summary>
    /// Gets or sets the presigned URL expiry in minutes
    /// </summary>
    public int PresignedUrlExpiryMinutes { get; set; } = 60;
}
