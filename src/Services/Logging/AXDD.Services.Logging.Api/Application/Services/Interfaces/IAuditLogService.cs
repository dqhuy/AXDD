using AXDD.Services.Logging.Api.Application.DTOs;

namespace AXDD.Services.Logging.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for audit log operations
/// </summary>
public interface IAuditLogService
{
    /// <summary>
    /// Creates a new audit log entry
    /// </summary>
    Task<AuditLogDto> CreateLogAsync(CreateAuditLogRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs with filtering and pagination
    /// </summary>
    Task<PagedResult<AuditLogDto>> GetLogsAsync(LogFilterDto filter, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets an audit log by ID
    /// </summary>
    Task<AuditLogDto> GetLogByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs by user ID
    /// </summary>
    Task<PagedResult<AuditLogDto>> GetLogsByUserAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs by service name
    /// </summary>
    Task<PagedResult<AuditLogDto>> GetLogsByServiceAsync(string serviceName, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs by date range
    /// </summary>
    Task<PagedResult<AuditLogDto>> GetLogsByDateRangeAsync(DateTime startDate, DateTime endDate, int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets audit logs by correlation ID for tracing requests across services
    /// </summary>
    Task<List<AuditLogDto>> GetLogsByCorrelationIdAsync(string correlationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes audit logs older than the specified number of days
    /// </summary>
    Task<int> DeleteOldLogsAsync(int olderThanDays, CancellationToken cancellationToken = default);
}
