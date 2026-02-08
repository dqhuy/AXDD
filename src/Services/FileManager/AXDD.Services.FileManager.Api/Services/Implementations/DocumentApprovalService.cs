using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Entities;
using AXDD.Services.FileManager.Api.Enums;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for document approval operations
/// </summary>
public class DocumentApprovalService : IDocumentApprovalService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<DocumentApprovalService> _logger;

    public DocumentApprovalService(FileManagerDbContext context, ILogger<DocumentApprovalService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentApprovalDto>> SubmitForApprovalAsync(SubmitApprovalRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            var document = await _context.FileMetadata
                .FirstOrDefaultAsync(f => f.Id == request.DocumentId && !f.IsDeleted, cancellationToken);

            if (document == null)
            {
                return Result<DocumentApprovalDto>.Failure($"Document with ID '{request.DocumentId}' not found");
            }

            // Check if there's already a pending approval for this document
            var existingPending = await _context.DocumentApprovals
                .AnyAsync(a => a.DocumentId == request.DocumentId && a.Status == ApprovalStatus.Pending && !a.IsDeleted, cancellationToken);

            if (existingPending)
            {
                return Result<DocumentApprovalDto>.Failure("There is already a pending approval request for this document");
            }

            var approval = new DocumentApproval
            {
                DocumentId = request.DocumentId,
                RequestedBy = userId,
                RequestedAt = DateTime.UtcNow,
                Status = ApprovalStatus.Pending,
                Notes = request.Notes,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.DocumentApprovals.Add(approval);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document submitted for approval: DocumentId={DocumentId}, RequestedBy={UserId}", request.DocumentId, userId);

            return Result<DocumentApprovalDto>.Success(MapToDto(approval, document.FileName));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting document for approval: {DocumentId}", request.DocumentId);
            return Result<DocumentApprovalDto>.Failure($"Failed to submit for approval: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentApprovalDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var approval = await _context.DocumentApprovals
                .Include(a => a.Document)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);

            if (approval == null)
            {
                return Result<DocumentApprovalDto>.Failure($"Approval with ID '{id}' not found");
            }

            return Result<DocumentApprovalDto>.Success(MapToDto(approval, approval.Document?.FileName ?? "Unknown"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting approval: {Id}", id);
            return Result<DocumentApprovalDto>.Failure($"Failed to get approval: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<DocumentApprovalDto>>> ListAsync(ApprovalStatus? status = null, string? requestedBy = null, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.DocumentApprovals
                .Include(a => a.Document)
                .Where(a => !a.IsDeleted);

            if (status.HasValue)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            if (!string.IsNullOrWhiteSpace(requestedBy))
            {
                query = query.Where(a => a.RequestedBy == requestedBy);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(a => a.RequestedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResult<DocumentApprovalDto>
            {
                Items = items.Select(a => MapToDto(a, a.Document?.FileName ?? "Unknown")).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResult<DocumentApprovalDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing approvals");
            return Result<PagedResult<DocumentApprovalDto>>.Failure($"Failed to list approvals: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentApprovalDto>> ApproveAsync(Guid id, ProcessApprovalRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            var approval = await _context.DocumentApprovals
                .Include(a => a.Document)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);

            if (approval == null)
            {
                return Result<DocumentApprovalDto>.Failure($"Approval with ID '{id}' not found");
            }

            if (approval.Status != ApprovalStatus.Pending)
            {
                return Result<DocumentApprovalDto>.Failure("Only pending approvals can be approved");
            }

            approval.Status = ApprovalStatus.Approved;
            approval.ApprovedBy = userId;
            approval.ApprovedAt = DateTime.UtcNow;
            approval.Notes = request?.Notes ?? approval.Notes;
            approval.UpdatedAt = DateTime.UtcNow;
            approval.UpdatedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document approved: ApprovalId={Id}, ApprovedBy={UserId}", id, userId);

            return Result<DocumentApprovalDto>.Success(MapToDto(approval, approval.Document?.FileName ?? "Unknown"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving document: {Id}", id);
            return Result<DocumentApprovalDto>.Failure($"Failed to approve document: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentApprovalDto>> RejectAsync(Guid id, ProcessApprovalRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);

        try
        {
            var approval = await _context.DocumentApprovals
                .Include(a => a.Document)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);

            if (approval == null)
            {
                return Result<DocumentApprovalDto>.Failure($"Approval with ID '{id}' not found");
            }

            if (approval.Status != ApprovalStatus.Pending)
            {
                return Result<DocumentApprovalDto>.Failure("Only pending approvals can be rejected");
            }

            approval.Status = ApprovalStatus.Rejected;
            approval.ApprovedBy = userId;
            approval.ApprovedAt = DateTime.UtcNow;
            approval.RejectionReason = request?.RejectionReason;
            approval.Notes = request?.Notes ?? approval.Notes;
            approval.UpdatedAt = DateTime.UtcNow;
            approval.UpdatedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document rejected: ApprovalId={Id}, RejectedBy={UserId}", id, userId);

            return Result<DocumentApprovalDto>.Success(MapToDto(approval, approval.Document?.FileName ?? "Unknown"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting document: {Id}", id);
            return Result<DocumentApprovalDto>.Failure($"Failed to reject document: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<DocumentApprovalDto>>> GetPendingApprovalsAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var approvals = await _context.DocumentApprovals
                .Include(a => a.Document)
                .Where(a => a.Status == ApprovalStatus.Pending && !a.IsDeleted)
                .OrderByDescending(a => a.RequestedAt)
                .ToListAsync(cancellationToken);

            return Result<List<DocumentApprovalDto>>.Success(
                approvals.Select(a => MapToDto(a, a.Document?.FileName ?? "Unknown")).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending approvals");
            return Result<List<DocumentApprovalDto>>.Failure($"Failed to get pending approvals: {ex.Message}");
        }
    }

    private static DocumentApprovalDto MapToDto(DocumentApproval approval, string documentName) => new()
    {
        Id = approval.Id,
        DocumentId = approval.DocumentId,
        DocumentName = documentName,
        RequestedBy = approval.RequestedBy,
        RequestedAt = approval.RequestedAt,
        Status = approval.Status,
        ApprovedBy = approval.ApprovedBy,
        ApprovedAt = approval.ApprovedAt,
        RejectionReason = approval.RejectionReason,
        Notes = approval.Notes,
        CreatedAt = approval.CreatedAt
    };
}
