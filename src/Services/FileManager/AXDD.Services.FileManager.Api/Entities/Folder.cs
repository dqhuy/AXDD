using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a folder in the file hierarchy
/// </summary>
public class Folder : BaseEntity
{
    public Folder()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the folder name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the parent folder ID for hierarchy
    /// </summary>
    public Guid? ParentFolderId { get; set; }

    /// <summary>
    /// Gets or sets the parent folder
    /// </summary>
    public Folder? ParentFolder { get; set; }

    /// <summary>
    /// Gets or sets the enterprise code this folder belongs to
    /// </summary>
    public string EnterpriseCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full path of the folder
    /// </summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the folder description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the folder type ID
    /// </summary>
    public Guid? FolderTypeId { get; set; }

    /// <summary>
    /// Gets or sets the folder type
    /// </summary>
    public FolderType? FolderType { get; set; }

    /// <summary>
    /// Gets or sets the digital storage ID
    /// </summary>
    public Guid? DigitalStorageId { get; set; }

    /// <summary>
    /// Gets or sets the digital storage
    /// </summary>
    public DigitalStorage? DigitalStorage { get; set; }

    /// <summary>
    /// Gets or sets the security level ID
    /// </summary>
    public Guid? SecurityLevelId { get; set; }

    /// <summary>
    /// Gets or sets the security level
    /// </summary>
    public SecurityLevel? SecurityLevel { get; set; }

    /// <summary>
    /// Gets or sets the password hash for protected folders
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// Gets or sets whether the folder is password protected
    /// </summary>
    public bool IsPasswordProtected { get; set; }

    /// <summary>
    /// Gets or sets the QR code data
    /// </summary>
    public string? QRCode { get; set; }

    /// <summary>
    /// Gets or sets the expiry date for time-limited folders
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Gets or sets the reminder date for notifications
    /// </summary>
    public DateTime? ReminderDate { get; set; }

    /// <summary>
    /// Gets or sets the collection of child folders
    /// </summary>
    public ICollection<Folder> ChildFolders { get; set; } = new List<Folder>();

    /// <summary>
    /// Gets or sets the collection of files in this folder
    /// </summary>
    public ICollection<FileMetadata> Files { get; set; } = new List<FileMetadata>();

    /// <summary>
    /// Gets or sets the collection of folder permissions
    /// </summary>
    public ICollection<FolderPermission> Permissions { get; set; } = new List<FolderPermission>();
}
