using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a metadata field definition for a folder type
/// </summary>
public class FolderTypeMetadataField : BaseEntity
{
    public FolderTypeMetadataField()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the folder type ID
    /// </summary>
    public Guid FolderTypeId { get; set; }

    /// <summary>
    /// Gets or sets the folder type
    /// </summary>
    public FolderType? FolderType { get; set; }

    /// <summary>
    /// Gets or sets the field name (internal use)
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name (user-facing)
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the data type (Text, Number, Date, List, Boolean)
    /// </summary>
    public string DataType { get; set; } = "Text";

    /// <summary>
    /// Gets or sets whether the field is required
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets the default value
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    /// Gets or sets the validation rules (JSON format)
    /// </summary>
    public string? ValidationRules { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the list options (for List data type, comma-separated)
    /// </summary>
    public string? ListOptions { get; set; }
}
