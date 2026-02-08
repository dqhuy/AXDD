using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Enums;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for document approval operations
/// </summary>
public interface IDocumentApprovalService
{
    /// <summary>
    /// Submits a document for approval
    /// </summary>
    Task<Result<DocumentApprovalDto>> SubmitForApprovalAsync(SubmitApprovalRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an approval by ID
    /// </summary>
    Task<Result<DocumentApprovalDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists approvals with filters
    /// </summary>
    Task<Result<PagedResult<DocumentApprovalDto>>> ListAsync(ApprovalStatus? status = null, string? requestedBy = null, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Approves a document
    /// </summary>
    Task<Result<DocumentApprovalDto>> ApproveAsync(Guid id, ProcessApprovalRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Rejects a document
    /// </summary>
    Task<Result<DocumentApprovalDto>> RejectAsync(Guid id, ProcessApprovalRequest request, string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets pending approvals for a user
    /// </summary>
    Task<Result<List<DocumentApprovalDto>>> GetPendingApprovalsAsync(string userId, CancellationToken cancellationToken = default);
}
