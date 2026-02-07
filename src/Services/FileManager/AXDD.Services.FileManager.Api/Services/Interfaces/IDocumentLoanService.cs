using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Enums;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for document loan operations
/// </summary>
public interface IDocumentLoanService
{
    /// <summary>
    /// Creates a new loan request
    /// </summary>
    Task<Result<DocumentLoanDto>> CreateAsync(CreateDocumentLoanRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a loan by ID
    /// </summary>
    Task<Result<DocumentLoanDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a loan by loan code
    /// </summary>
    Task<Result<DocumentLoanDto>> GetByCodeAsync(string loanCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists loans with filters
    /// </summary>
    Task<Result<PagedResult<DocumentLoanDto>>> ListAsync(
        string? enterpriseCode = null,
        LoanStatus? status = null,
        LoanType? loanType = null,
        string? borrowerUserId = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Approves a loan request
    /// </summary>
    Task<Result<DocumentLoanDto>> ApproveAsync(Guid id, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rejects a loan request
    /// </summary>
    Task<Result<DocumentLoanDto>> RejectAsync(Guid id, ProcessLoanRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks a loan as borrowed (documents handed over)
    /// </summary>
    Task<Result<DocumentLoanDto>> MarkAsBorrowedAsync(Guid id, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns documents for a loan
    /// </summary>
    Task<Result<DocumentLoanDto>> ReturnDocumentsAsync(Guid id, List<Guid>? itemIds = null, string userId = "", CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets loan statistics
    /// </summary>
    Task<Result<LoanStatisticsDto>> GetStatisticsAsync(string? enterpriseCode = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets overdue loans
    /// </summary>
    Task<Result<List<DocumentLoanDto>>> GetOverdueLoansAsync(string? enterpriseCode = null, CancellationToken cancellationToken = default);
}
