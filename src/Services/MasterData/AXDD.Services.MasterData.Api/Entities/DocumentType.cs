using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.MasterData.Api.Entities;

/// <summary>
/// Represents a type of document
/// </summary>
public class DocumentType : BaseEntity
{
    /// <summary>
    /// Gets or sets the document type code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the document type name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category (Investment, Legal, Financial, etc.)
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether this document is required
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets the allowed file extensions (comma-separated)
    /// </summary>
    public string? AllowedExtensions { get; set; }

    /// <summary>
    /// Gets or sets the maximum file size in MB
    /// </summary>
    public int? MaxFileSizeMB { get; set; }

    /// <summary>
    /// Gets or sets whether this document type is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}
