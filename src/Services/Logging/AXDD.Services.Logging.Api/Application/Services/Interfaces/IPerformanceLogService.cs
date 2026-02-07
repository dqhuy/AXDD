using AXDD.Services.Logging.Api.Application.DTOs;

namespace AXDD.Services.Logging.Api.Application.Services.Interfaces;

/// <summary>
/// Service interface for performance log operations
/// </summary>
public interface IPerformanceLogService
{
    /// <summary>
    /// Logs performance metrics
    /// </summary>
    Task<PerformanceLogDto> LogPerformanceAsync(CreatePerformanceLogRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets performance logs with filtering and pagination
    /// </summary>
    Task<PagedResult<PerformanceLogDto>> GetPerformanceLogsAsync(int pageNumber, int pageSize, string? serviceName = null, DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets service statistics
    /// </summary>
    Task<LogStatisticsDto> GetServiceStatisticsAsync(string serviceName, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets slow requests above threshold
    /// </summary>
    Task<PagedResult<PerformanceLogDto>> GetSlowRequestsAsync(long thresholdMs, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
}
