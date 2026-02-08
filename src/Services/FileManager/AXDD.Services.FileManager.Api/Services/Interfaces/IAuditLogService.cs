using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Enums;

namespace AXDD.Services.FileManager.Api.Services.Interfaces;

/// <summary>
/// Service interface for audit log operations
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Logs an action
    /// </summary>
    Task<Result> LogAsync(
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
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs with filters
    /// </summary>
    Task<Result<PagedResult<AuditLogDto>>> ListAsync(AuditLogQueryDto query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets access logs
    /// </summary>
    Task<Result<PagedResult<AuditLogDto>>> GetAccessLogsAsync(string? enterpriseCode = null, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets login logs
    /// </summary>
    Task<Result<PagedResult<AuditLogDto>>> GetLoginLogsAsync(string? enterpriseCode = null, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets content change logs
    /// </summary>
    Task<Result<PagedResult<AuditLogDto>>> GetChangeLogsAsync(string? enterpriseCode = null, string? entityType = null, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes logs older than specified date
    /// </summary>
    Task<Result<int>> DeleteLogsOlderThanAsync(DateTime date, string userId, CancellationToken cancellationToken = default);
}
