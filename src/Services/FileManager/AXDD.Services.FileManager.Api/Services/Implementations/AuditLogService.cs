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
/// Service implementation for audit log operations
/// </summary>
public class AuditLogService : IAuditLogService
{
    private readonly FileManagerDbContext _context;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(FileManagerDbContext context, ILogger<AuditLogService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Result> LogAsync(
        string userId,
        string userName,
        AuditAction action,
        string entityType,
        string entityId,
        string? entityName = null,
        string? oldValue = null,
        string? newValue = null,
        string? ipAddress = null,
        string? userAgent = null,
        string enterpriseCode = "",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var auditLog = new AuditLog
            {
                UserId = userId,
                UserName = userName,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                EntityName = entityName,
                OldValue = oldValue,
                NewValue = newValue,
                IpAddress = ipAddress,
                UserAgent = userAgent,
                Timestamp = DateTime.UtcNow,
                EnterpriseCode = enterpriseCode,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating audit log for {Action} on {EntityType}", action, entityType);
            return Result.Failure($"Failed to create audit log: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<AuditLogDto>>> ListAsync(AuditLogQueryDto query, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        try
        {
            var queryable = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.EnterpriseCode))
            {
                queryable = queryable.Where(a => a.EnterpriseCode == query.EnterpriseCode);
            }

            if (!string.IsNullOrWhiteSpace(query.UserId))
            {
                queryable = queryable.Where(a => a.UserId == query.UserId);
            }

            if (query.Action.HasValue)
            {
                queryable = queryable.Where(a => a.Action == query.Action.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.EntityType))
            {
                queryable = queryable.Where(a => a.EntityType == query.EntityType);
            }

            if (query.FromDate.HasValue)
            {
                queryable = queryable.Where(a => a.Timestamp >= query.FromDate.Value);
            }

            if (query.ToDate.HasValue)
            {
                queryable = queryable.Where(a => a.Timestamp <= query.ToDate.Value);
            }

            var totalCount = await queryable.CountAsync(cancellationToken);

            var items = await queryable
                .OrderByDescending(a => a.Timestamp)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResult<AuditLogDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            return Result<PagedResult<AuditLogDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error listing audit logs");
            return Result<PagedResult<AuditLogDto>>.Failure($"Failed to list audit logs: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<AuditLogDto>>> GetAccessLogsAsync(string? enterpriseCode = null, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var query = new AuditLogQueryDto
        {
            EnterpriseCode = enterpriseCode,
            Action = AuditAction.Read,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        return await ListAsync(query, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<AuditLogDto>>> GetLoginLogsAsync(string? enterpriseCode = null, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            var queryable = _context.AuditLogs
                .Where(a => a.Action == AuditAction.Login || a.Action == AuditAction.Logout);

            if (!string.IsNullOrWhiteSpace(enterpriseCode))
            {
                queryable = queryable.Where(a => a.EnterpriseCode == enterpriseCode);
            }

            var totalCount = await queryable.CountAsync(cancellationToken);

            var items = await queryable
                .OrderByDescending(a => a.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResult<AuditLogDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResult<AuditLogDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting login logs");
            return Result<PagedResult<AuditLogDto>>.Failure($"Failed to get login logs: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<PagedResult<AuditLogDto>>> GetChangeLogsAsync(string? enterpriseCode = null, string? entityType = null, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        try
        {
            var queryable = _context.AuditLogs
                .Where(a => a.Action == AuditAction.Create || a.Action == AuditAction.Update || a.Action == AuditAction.Delete);

            if (!string.IsNullOrWhiteSpace(enterpriseCode))
            {
                queryable = queryable.Where(a => a.EnterpriseCode == enterpriseCode);
            }

            if (!string.IsNullOrWhiteSpace(entityType))
            {
                queryable = queryable.Where(a => a.EntityType == entityType);
            }

            var totalCount = await queryable.CountAsync(cancellationToken);

            var items = await queryable
                .OrderByDescending(a => a.Timestamp)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var result = new PagedResult<AuditLogDto>
            {
                Items = items.Select(MapToDto).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Result<PagedResult<AuditLogDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting change logs");
            return Result<PagedResult<AuditLogDto>>.Failure($"Failed to get change logs: {ex.Message}");
        }
    }

    /// <inheritdoc/>
    public async Task<Result<int>> DeleteLogsOlderThanAsync(DateTime date, string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var logsToDelete = await _context.AuditLogs
                .Where(a => a.Timestamp < date)
                .ToListAsync(cancellationToken);

            var count = logsToDelete.Count;

            if (count > 0)
            {
                _context.AuditLogs.RemoveRange(logsToDelete);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Deleted {Count} audit logs older than {Date} by user {UserId}", count, date, userId);
            }

            return Result<int>.Success(count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting audit logs older than {Date}", date);
            return Result<int>.Failure($"Failed to delete audit logs: {ex.Message}");
        }
    }

    private static AuditLogDto MapToDto(AuditLog entity) => new()
    {
        Id = entity.Id,
        UserId = entity.UserId,
        UserName = entity.UserName,
        Action = entity.Action,
        EntityType = entity.EntityType,
        EntityId = entity.EntityId,
        EntityName = entity.EntityName,
        OldValue = entity.OldValue,
        NewValue = entity.NewValue,
        IpAddress = entity.IpAddress,
        UserAgent = entity.UserAgent,
        Timestamp = entity.Timestamp,
        EnterpriseCode = entity.EnterpriseCode
    };
}
