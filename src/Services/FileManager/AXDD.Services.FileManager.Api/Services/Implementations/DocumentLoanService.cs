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
/// Service implementation for document loan operations
/// </summary>
public class DocumentLoanService : IDocumentLoanService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<DocumentLoanService> _logger;

    public DocumentLoanService(FileManagerDbContext context, ILogger<DocumentLoanService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentLoanDto>> CreateAsync(CreateDocumentLoanRequest request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.BorrowerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EnterpriseCode);

        if (request.DocumentIds == null || request.DocumentIds.Count == 0)
        {
            return Result<DocumentLoanDto>.Failure("At least one document is required");
        }

        try
        {
            // Generate loan code
            var loanCode = $"LOAN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

            // Verify all documents exist
            var documents = await _context.FileMetadata
                .Where(f => request.DocumentIds.Contains(f.Id) && !f.IsDeleted)
                .ToListAsync(cancellationToken);

            if (documents.Count != request.DocumentIds.Count)
            {
                return Result<DocumentLoanDto>.Failure("One or more documents not found");
            }

            var loan = new DocumentLoan
            {
                LoanCode = loanCode,
                BorrowerUserId = userId,
                BorrowerName = request.BorrowerName,
                BorrowerDepartment = request.BorrowerDepartment,
                RequestedAt = DateTime.UtcNow,
                DueDate = request.DueDate,
                Status = LoanStatus.Pending,
                LoanType = request.LoanType,
                Purpose = request.Purpose,
                EnterpriseCode = request.EnterpriseCode,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            // Add loan items
            foreach (var doc in documents)
            {
                loan.Items.Add(new DocumentLoanItem
                {
                    DocumentId = doc.Id,
                    DocumentName = doc.FileName,
                    IsReturned = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                });
            }

            _context.DocumentLoans.Add(loan);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document loan created: {LoanCode}", loanCode);

            return Result<DocumentLoanDto>.Success(MapToDto(loan));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating document loan");
            return Result<DocumentLoanDto>.Failure($"Failed to create document loan: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentLoanDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var loan = await _context.DocumentLoans
                .Include(l => l.Items)
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, cancellationToken);

            if (loan == null)
            {
                return Result<DocumentLoanDto>.Failure($"Document loan with ID '{id}' not found");
            }

            return Result<DocumentLoanDto>.Success(MapToDto(loan));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document loan: {Id}", id);
            return Result<DocumentLoanDto>.Failure($"Failed to get document loan: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentLoanDto>> GetByCodeAsync(string loanCode, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(loanCode);

        try
        {
            var loan = await _context.DocumentLoans
                .Include(l => l.Items)
                .FirstOrDefaultAsync(l => l.LoanCode == loanCode && !l.IsDeleted, cancellationToken);

            if (loan == null)
            {
                return Result<DocumentLoanDto>.Failure($"Document loan with code '{loanCode}' not found");
            }

            return Result<DocumentLoanDto>.Success(MapToDto(loan));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document loan by code: {LoanCode}", loanCode);
            return Result<DocumentLoanDto>.Failure($"Failed to get document loan: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<DocumentLoanDto>>> ListAsync(
        string? enterpriseCode = null,
        LoanStatus? status = null,
        LoanType? loanType = null,
        string? borrowerUserId = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.DocumentLoans
                .Include(l => l.Items)
                .Where(l => !l.IsDeleted);

            if (!string.IsNullOrWhiteSpace(enterpriseCode))
            {
                query = query.Where(l => l.EnterpriseCode == enterpriseCode);
            }

            if (status.HasValue)
            {
                query = query.Where(l => l.Status == status.Value);
            }

            if (loanType.HasValue)
            {
                query = query.Where(l => l.LoanType == loanType.Value);
            }

            if (!string.IsNullOrWhiteSpace(borrowerUserId))
            {
                query = query.Where(l => l.BorrowerUserId == borrowerUserId);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(l => l.RequestedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResult<DocumentLoanDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResult<DocumentLoanDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing document loans");
            return Result<PagedResult<DocumentLoanDto>>.Failure($"Failed to list document loans: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentLoanDto>> ApproveAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var loan = await _context.DocumentLoans
                .Include(l => l.Items)
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, cancellationToken);

            if (loan == null)
            {
                return Result<DocumentLoanDto>.Failure($"Document loan with ID '{id}' not found");
            }

            if (loan.Status != LoanStatus.Pending)
            {
                return Result<DocumentLoanDto>.Failure("Only pending loans can be approved");
            }

            loan.Status = LoanStatus.Approved;
            loan.ApprovedBy = userId;
            loan.ApprovedAt = DateTime.UtcNow;
            loan.UpdatedAt = DateTime.UtcNow;
            loan.UpdatedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document loan approved: {Id}", id);

            return Result<DocumentLoanDto>.Success(MapToDto(loan));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving document loan: {Id}", id);
            return Result<DocumentLoanDto>.Failure($"Failed to approve document loan: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentLoanDto>> RejectAsync(Guid id, ProcessLoanRequest request, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var loan = await _context.DocumentLoans
                .Include(l => l.Items)
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, cancellationToken);

            if (loan == null)
            {
                return Result<DocumentLoanDto>.Failure($"Document loan with ID '{id}' not found");
            }

            if (loan.Status != LoanStatus.Pending)
            {
                return Result<DocumentLoanDto>.Failure("Only pending loans can be rejected");
            }

            loan.Status = LoanStatus.Rejected;
            loan.RejectionReason = request?.RejectionReason;
            loan.UpdatedAt = DateTime.UtcNow;
            loan.UpdatedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document loan rejected: {Id}", id);

            return Result<DocumentLoanDto>.Success(MapToDto(loan));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting document loan: {Id}", id);
            return Result<DocumentLoanDto>.Failure($"Failed to reject document loan: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentLoanDto>> MarkAsBorrowedAsync(Guid id, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var loan = await _context.DocumentLoans
                .Include(l => l.Items)
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, cancellationToken);

            if (loan == null)
            {
                return Result<DocumentLoanDto>.Failure($"Document loan with ID '{id}' not found");
            }

            if (loan.Status != LoanStatus.Approved)
            {
                return Result<DocumentLoanDto>.Failure("Only approved loans can be marked as borrowed");
            }

            loan.Status = LoanStatus.Borrowed;
            loan.UpdatedAt = DateTime.UtcNow;
            loan.UpdatedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Document loan marked as borrowed: {Id}", id);

            return Result<DocumentLoanDto>.Success(MapToDto(loan));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking document loan as borrowed: {Id}", id);
            return Result<DocumentLoanDto>.Failure($"Failed to mark document loan as borrowed: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentLoanDto>> ReturnDocumentsAsync(Guid id, List<Guid>? itemIds = null, string userId = "", CancellationToken cancellationToken = default)
    {
        try
        {
            var loan = await _context.DocumentLoans
                .Include(l => l.Items)
                .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, cancellationToken);

            if (loan == null)
            {
                return Result<DocumentLoanDto>.Failure($"Document loan with ID '{id}' not found");
            }

            if (loan.Status != LoanStatus.Borrowed && loan.Status != LoanStatus.Overdue)
            {
                return Result<DocumentLoanDto>.Failure("Only borrowed or overdue loans can be returned");
            }

            var now = DateTime.UtcNow;

            // Return specific items or all items
            var itemsToReturn = itemIds != null && itemIds.Count > 0
                ? loan.Items.Where(i => itemIds.Contains(i.Id)).ToList()
                : loan.Items.ToList();

            foreach (var item in itemsToReturn)
            {
                item.IsReturned = true;
                item.ReturnedAt = now;
                item.UpdatedAt = now;
                item.UpdatedBy = userId;
            }

            // Check if all items are returned
            if (loan.Items.All(i => i.IsReturned))
            {
                loan.Status = LoanStatus.Returned;
                loan.ReturnedAt = now;
            }

            loan.UpdatedAt = now;
            loan.UpdatedBy = userId;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Documents returned for loan: {Id}", id);

            return Result<DocumentLoanDto>.Success(MapToDto(loan));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error returning documents for loan: {Id}", id);
            return Result<DocumentLoanDto>.Failure($"Failed to return documents: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<LoanStatisticsDto>> GetStatisticsAsync(string? enterpriseCode = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.DocumentLoans.Where(l => !l.IsDeleted);

            if (!string.IsNullOrWhiteSpace(enterpriseCode))
            {
                query = query.Where(l => l.EnterpriseCode == enterpriseCode);
            }

            var loans = await query.ToListAsync(cancellationToken);

            var stats = new LoanStatisticsDto
            {
                TotalLoans = loans.Count,
                PendingLoans = loans.Count(l => l.Status == LoanStatus.Pending),
                ApprovedLoans = loans.Count(l => l.Status == LoanStatus.Approved),
                RejectedLoans = loans.Count(l => l.Status == LoanStatus.Rejected),
                BorrowedLoans = loans.Count(l => l.Status == LoanStatus.Borrowed),
                ReturnedLoans = loans.Count(l => l.Status == LoanStatus.Returned),
                OverdueLoans = loans.Count(l => l.Status == LoanStatus.Overdue || (l.Status == LoanStatus.Borrowed && l.DueDate < DateTime.UtcNow)),
                LoansByType = loans.GroupBy(l => l.LoanType).ToDictionary(g => g.Key, g => g.Count()),
                TotalBorrowers = loans.Select(l => l.BorrowerUserId).Distinct().Count()
            };

            return Result<LoanStatisticsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting loan statistics");
            return Result<LoanStatisticsDto>.Failure($"Failed to get loan statistics: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<List<DocumentLoanDto>>> GetOverdueLoansAsync(string? enterpriseCode = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _context.DocumentLoans
                .Include(l => l.Items)
                .Where(l => !l.IsDeleted && l.Status == LoanStatus.Borrowed && l.DueDate < DateTime.UtcNow);

            if (!string.IsNullOrWhiteSpace(enterpriseCode))
            {
                query = query.Where(l => l.EnterpriseCode == enterpriseCode);
            }

            var loans = await query
                .OrderBy(l => l.DueDate)
                .ToListAsync(cancellationToken);

            // Update status to overdue
            foreach (var loan in loans)
            {
                if (loan.Status == LoanStatus.Borrowed)
                {
                    loan.Status = LoanStatus.Overdue;
                }
            }

            if (loans.Count > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            return Result<List<DocumentLoanDto>>.Success(loans.Select(MapToDto).ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting overdue loans");
            return Result<List<DocumentLoanDto>>.Failure($"Failed to get overdue loans: {ex.Message}");
        }
    }

    private static DocumentLoanDto MapToDto(DocumentLoan loan) => new()
    {
        Id = loan.Id,
        LoanCode = loan.LoanCode,
        BorrowerUserId = loan.BorrowerUserId,
        BorrowerName = loan.BorrowerName,
        BorrowerDepartment = loan.BorrowerDepartment,
        RequestedAt = loan.RequestedAt,
        DueDate = loan.DueDate,
        ReturnedAt = loan.ReturnedAt,
        Status = loan.Status,
        LoanType = loan.LoanType,
        Purpose = loan.Purpose,
        ApprovedBy = loan.ApprovedBy,
        ApprovedAt = loan.ApprovedAt,
        RejectionReason = loan.RejectionReason,
        EnterpriseCode = loan.EnterpriseCode,
        CreatedAt = loan.CreatedAt,
        Items = loan.Items?.Select(i => new DocumentLoanItemDto
        {
            Id = i.Id,
            DocumentLoanId = i.DocumentLoanId,
            DocumentId = i.DocumentId,
            DocumentName = i.DocumentName,
            IsReturned = i.IsReturned,
            ReturnedAt = i.ReturnedAt,
            Notes = i.Notes
        }).ToList() ?? []
    };
}
