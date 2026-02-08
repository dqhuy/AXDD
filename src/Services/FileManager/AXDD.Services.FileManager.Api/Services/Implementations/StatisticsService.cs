using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.Data;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Enums;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.FileManager.Api.Services.Implementations;

/// <summary>
/// Service implementation for document management statistics
/// </summary>
public class StatisticsService : IStatisticsService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<StatisticsService> _logger;

    public StatisticsService(FileManagerDbContext context, ILogger<StatisticsService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result<DocumentStatisticsDto>> GetDocumentStatisticsAsync(string? enterpriseCode = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var documentQuery = _context.FileMetadata.Where(f => !f.IsDeleted);
            var approvalQuery = _context.DocumentApprovals.Where(a => !a.IsDeleted);

            if (!string.IsNullOrWhiteSpace(enterpriseCode))
            {
                documentQuery = documentQuery.Where(f => f.EnterpriseCode == enterpriseCode);
            }

            var totalDocuments = await documentQuery.CountAsync(cancellationToken);
            
            // Count documents with OCR content (digitized)
            var digitizedDocuments = await documentQuery
                .CountAsync(f => !string.IsNullOrEmpty(f.OcrContent), cancellationToken);

            // Documents by type
            var documentsByType = await documentQuery
                .Where(f => f.DocumentTypeId != null)
                .Include(f => f.DocumentType)
                .GroupBy(f => f.DocumentType!.Name)
                .Select(g => new { TypeName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TypeName, x => x.Count, cancellationToken);

            // Approved/Rejected counts
            var approvedDocuments = await approvalQuery
                .CountAsync(a => a.Status == ApprovalStatus.Approved, cancellationToken);
            var rejectedDocuments = await approvalQuery
                .CountAsync(a => a.Status == ApprovalStatus.Rejected, cancellationToken);
            var pendingCataloging = await approvalQuery
                .CountAsync(a => a.Status == ApprovalStatus.Pending, cancellationToken);

            // Storage statistics
            var totalStorageBytes = await _context.DigitalStorages
                .Where(s => !s.IsDeleted)
                .SumAsync(s => s.TotalCapacityBytes, cancellationToken);

            var usedStorageBytes = await _context.DigitalStorages
                .Where(s => !s.IsDeleted)
                .SumAsync(s => s.UsedCapacityBytes, cancellationToken);

            var stats = new DocumentStatisticsDto
            {
                TotalDocuments = totalDocuments,
                DigitizedDocuments = digitizedDocuments,
                PendingCataloging = pendingCataloging,
                CatalogedDocuments = totalDocuments - pendingCataloging,
                ApprovedDocuments = approvedDocuments,
                RejectedDocuments = rejectedDocuments,
                DocumentsByType = documentsByType,
                TotalStorageBytes = totalStorageBytes,
                UsedStorageBytes = usedStorageBytes
            };

            return Result<DocumentStatisticsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document statistics");
            return Result<DocumentStatisticsDto>.Failure($"Failed to get document statistics: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<StorageStatisticsDto>> GetStorageStatisticsAsync(string? enterpriseCode = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var folderQuery = _context.Folders.Where(f => !f.IsDeleted);
            var storageQuery = _context.DigitalStorages.Where(s => !s.IsDeleted);

            if (!string.IsNullOrWhiteSpace(enterpriseCode))
            {
                folderQuery = folderQuery.Where(f => f.EnterpriseCode == enterpriseCode);
                storageQuery = storageQuery.Where(s => s.EnterpriseCode == enterpriseCode);
            }

            var totalFolders = await folderQuery.CountAsync(cancellationToken);

            // Folders by type
            var foldersByType = await folderQuery
                .Where(f => f.FolderTypeId != null)
                .Include(f => f.FolderType)
                .GroupBy(f => f.FolderType!.Name)
                .Select(g => new { TypeName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.TypeName, x => x.Count, cancellationToken);

            // Storage by enterprise
            var storageByEnterprise = await storageQuery
                .GroupBy(s => s.EnterpriseCode)
                .Select(g => new { EnterpriseCode = g.Key, Bytes = g.Sum(s => s.UsedCapacityBytes) })
                .ToDictionaryAsync(x => x.EnterpriseCode, x => x.Bytes, cancellationToken);

            // Active users (from audit log last 30 days)
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var activeUsers = await _context.AuditLogs
                .Where(a => a.Timestamp >= thirtyDaysAgo)
                .Select(a => a.UserId)
                .Distinct()
                .CountAsync(cancellationToken);

            var stats = new StorageStatisticsDto
            {
                TotalFolders = totalFolders,
                FoldersByType = foldersByType,
                StorageByEnterpriseBytes = storageByEnterprise,
                ActiveUsers = activeUsers
            };

            return Result<StorageStatisticsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting storage statistics");
            return Result<StorageStatisticsDto>.Failure($"Failed to get storage statistics: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<LoanStatisticsDto>> GetLoanStatisticsAsync(string? enterpriseCode = null, CancellationToken cancellationToken = default)
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
}
