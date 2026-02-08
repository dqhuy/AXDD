using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a folder type (e.g., project folder, employee folder)
/// </summary>
public class FolderType : BaseEntity
{
    public FolderType()
    {
        Id = Guid.NewGuid();
        MetadataFields = new List<FolderTypeMetadataField>();
    }

    /// <summary>
    /// Gets or sets the unique code of the folder type
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the folder type
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the folder type
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether the folder type is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the retention period in months (0 = permanent)
    /// </summary>
    public int RetentionPeriodMonths { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the collection of metadata fields for this folder type
    /// </summary>
    public ICollection<FolderTypeMetadataField> MetadataFields { get; set; }
}
