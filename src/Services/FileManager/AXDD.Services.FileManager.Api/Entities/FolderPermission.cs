using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.FileManager.Api.Enums;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a folder permission assignment
/// </summary>
public class FolderPermission : BaseEntity
{
    public FolderPermission()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the folder ID
    /// </summary>
    public Guid FolderId { get; set; }

    /// <summary>
    /// Gets or sets the folder
    /// </summary>
    public Folder? Folder { get; set; }

    /// <summary>
    /// Gets or sets the user ID (null if permission is for a group)
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the user group ID (null if permission is for a user)
    /// </summary>
    public string? UserGroupId { get; set; }

    /// <summary>
    /// Gets or sets the permission level
    /// </summary>
    public PermissionLevel Permission { get; set; }

    /// <summary>
    /// Gets or sets whether the user can share the folder
    /// </summary>
    public bool CanShare { get; set; }

    /// <summary>
    /// Gets or sets whether the user can download files from the folder
    /// </summary>
    public bool CanDownload { get; set; }

    /// <summary>
    /// Gets or sets whether the user can print files from the folder
    /// </summary>
    public bool CanPrint { get; set; }

    /// <summary>
    /// Gets or sets when the permission expires (null = never)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets the user ID who granted the permission
    /// </summary>
    public string GrantedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when the permission was granted
    /// </summary>
    public DateTime GrantedAt { get; set; }
}
