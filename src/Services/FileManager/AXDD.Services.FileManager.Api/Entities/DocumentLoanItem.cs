using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents an item in a document loan
/// </summary>
public class DocumentLoanItem : BaseEntity
{
    public DocumentLoanItem()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the document loan ID
    /// </summary>
    public Guid DocumentLoanId { get; set; }

    /// <summary>
    /// Gets or sets the document loan
    /// </summary>
    public DocumentLoan? DocumentLoan { get; set; }

    /// <summary>
    /// Gets or sets the document (file) ID
    /// </summary>
    public Guid DocumentId { get; set; }

    /// <summary>
    /// Gets or sets the document
    /// </summary>
    public FileMetadata? Document { get; set; }

    /// <summary>
    /// Gets or sets the document name (snapshot at loan time)
    /// </summary>
    public string DocumentName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether this item has been returned
    /// </summary>
    public bool IsReturned { get; set; }

    /// <summary>
    /// Gets or sets when this item was returned
    /// </summary>
    public DateTime? ReturnedAt { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    public string? Notes { get; set; }
}
