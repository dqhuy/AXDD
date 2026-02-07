using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a document type (e.g., contract, invoice, report)
/// </summary>
public class DocumentType : BaseEntity
{
    public DocumentType()
    {
        Id = Guid.NewGuid();
        MetadataFields = new List<DocumentTypeMetadataField>();
    }

    /// <summary>
    /// Gets or sets the unique code of the document type
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the document type
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the document type
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether the document type is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets the collection of metadata fields for this document type
    /// </summary>
    public ICollection<DocumentTypeMetadataField> MetadataFields { get; set; }
}
