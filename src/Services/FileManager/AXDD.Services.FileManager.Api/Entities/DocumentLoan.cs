using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.FileManager.Api.Enums;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a document loan request
/// </summary>
public class DocumentLoan : BaseEntity
{
    public DocumentLoan()
    {
        Id = Guid.NewGuid();
        Items = new List<DocumentLoanItem>();
    }

    /// <summary>
    /// Gets or sets the loan code (unique identifier)
    /// </summary>
    public string LoanCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the borrower's user ID
    /// </summary>
    public string BorrowerUserId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the borrower's name
    /// </summary>
    public string BorrowerName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the borrower's department
    /// </summary>
    public string? BorrowerDepartment { get; set; }

    /// <summary>
    /// Gets or sets when the loan was requested
    /// </summary>
    public DateTime RequestedAt { get; set; }

    /// <summary>
    /// Gets or sets the due date for return
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Gets or sets when the documents were returned
    /// </summary>
    public DateTime? ReturnedAt { get; set; }

    /// <summary>
    /// Gets or sets the loan status
    /// </summary>
    public LoanStatus Status { get; set; } = LoanStatus.Pending;

    /// <summary>
    /// Gets or sets the loan type
    /// </summary>
    public LoanType LoanType { get; set; }

    /// <summary>
    /// Gets or sets the purpose of the loan
    /// </summary>
    public string? Purpose { get; set; }

    /// <summary>
    /// Gets or sets the user ID who approved the loan
    /// </summary>
    public string? ApprovedBy { get; set; }

    /// <summary>
    /// Gets or sets when the loan was approved
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// Gets or sets the rejection reason (if rejected)
    /// </summary>
    public string? RejectionReason { get; set; }

    /// <summary>
    /// Gets or sets the enterprise code
    /// </summary>
    public string EnterpriseCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of loan items
    /// </summary>
    public ICollection<DocumentLoanItem> Items { get; set; }
}
