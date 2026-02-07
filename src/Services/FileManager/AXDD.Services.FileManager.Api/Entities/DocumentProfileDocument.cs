using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a document within a document profile
/// Links a file (FileMetadata) to a profile with custom metadata
/// </summary>
public class DocumentProfileDocument : BaseEntity
{
    public DocumentProfileDocument()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the profile ID this document belongs to
    /// </summary>
    public Guid ProfileId { get; set; }

    /// <summary>
    /// Gets or sets the profile this document belongs to
    /// </summary>
    public DocumentProfile Profile { get; set; } = null!;

    /// <summary>
    /// Gets or sets the file metadata ID (linked file)
    /// </summary>
    public Guid FileMetadataId { get; set; }

    /// <summary>
    /// Gets or sets the file metadata
    /// </summary>
    public FileMetadata FileMetadata { get; set; } = null!;

    /// <summary>
    /// Gets or sets the document title/name (can be different from file name)
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the document description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the document type (e.g., "Giấy chứng nhận đầu tư", "Giấy phép môi trường")
    /// </summary>
    public string DocumentType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the document number/code
    /// </summary>
    public string? DocumentNumber { get; set; }

    /// <summary>
    /// Gets or sets the document issue date
    /// </summary>
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// Gets or sets the document expiry date
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Gets or sets the issuing authority
    /// </summary>
    public string? IssuingAuthority { get; set; }

    /// <summary>
    /// Gets or sets the document status (Draft, Active, Expired, Revoked)
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Gets or sets the display order within the profile
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Gets or sets the collection of custom metadata values for this document
    /// </summary>
    public ICollection<DocumentMetadataValue> MetadataValues { get; set; } = new List<DocumentMetadataValue>();
}
