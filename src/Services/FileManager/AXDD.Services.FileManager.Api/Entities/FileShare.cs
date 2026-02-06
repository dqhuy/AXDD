using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a file sharing permission
/// </summary>
public class FileShare : BaseEntity
{
    public FileShare()
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
    /// Gets or sets the user ID this file is shared with
    /// </summary>
    public string SharedWithUserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the permission level (Read, Write)
    /// </summary>
    public string Permission { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when this share expires
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets whether the share is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the user who shared the file
    /// </summary>
    public string SharedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the file was shared
    /// </summary>
    public DateTime SharedAt { get; set; }
}
