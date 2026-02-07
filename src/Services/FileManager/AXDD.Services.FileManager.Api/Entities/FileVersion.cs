using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a version of a file
/// </summary>
public class FileVersion : BaseEntity
{
    public FileVersion()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the file metadata ID
    /// </summary>
    public Guid FileMetadataId { get; set; }

    /// <summary>
    /// Gets or sets the file metadata
    /// </summary>
    public FileMetadata FileMetadata { get; set; } = null!;

    /// <summary>
    /// Gets or sets the version number
    /// </summary>
    public int Version { get; set; }

    /// <summary>
    /// Gets or sets the MinIO object key for this version
    /// </summary>
    public string ObjectKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the file size in bytes
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the user who uploaded this version
    /// </summary>
    public string UploadedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when this version was uploaded
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    /// Gets or sets the checksum for this version
    /// </summary>
    public string Checksum { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the version notes
    /// </summary>
    public string? Notes { get; set; }
}
