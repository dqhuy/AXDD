using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a metadata field definition for a document type
/// </summary>
public class DocumentTypeMetadataField : BaseEntity
{
    public DocumentTypeMetadataField()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the document type ID
    /// </summary>
    public Guid DocumentTypeId { get; set; }

    /// <summary>
    /// Gets or sets the document type
    /// </summary>
    public DocumentType? DocumentType { get; set; }

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
