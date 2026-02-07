using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.FileManager.Api.Enums;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a document approval request
/// </summary>
public class DocumentApproval : BaseEntity
{
    public DocumentApproval()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the document (file) ID being approved
    /// </summary>
    public Guid DocumentId { get; set; }

    /// <summary>
    /// Gets or sets the document
    /// </summary>
    public FileMetadata? Document { get; set; }

    /// <summary>
    /// Gets or sets the user ID who requested approval
    /// </summary>
    public string RequestedBy { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets when approval was requested
    /// </summary>
    public DateTime RequestedAt { get; set; }

    /// <summary>
    /// Gets or sets the approval status
    /// </summary>
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

    /// <summary>
    /// Gets or sets the user ID who approved/rejected
    /// </summary>
    public string? ApprovedBy { get; set; }

    /// <summary>
    /// Gets or sets when the approval was made
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// Gets or sets the rejection reason (if rejected)
    /// </summary>
    public string? RejectionReason { get; set; }

    /// <summary>
    /// Gets or sets additional notes
    /// </summary>
    public string? Notes { get; set; }
}
