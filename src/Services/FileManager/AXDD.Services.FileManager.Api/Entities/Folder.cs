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
    /// Gets or sets the collection of child folders
    /// </summary>
    public ICollection<Folder> ChildFolders { get; set; } = new List<Folder>();

    /// <summary>
    /// Gets or sets the collection of files in this folder
    /// </summary>
    public ICollection<FileMetadata> Files { get; set; } = new List<FileMetadata>();
}
