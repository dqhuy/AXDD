using AXDD.Services.Logging.Api.Application.DTOs;
using AXDD.Services.Logging.Api.Application.Services.Interfaces;
using AXDD.Services.Logging.Api.Domain.Entities;
using AXDD.Services.Logging.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Logging.Api.Application.Services;

/// <summary>
/// Service for performance log operations
/// </summary>
public class PerformanceLogService : IPerformanceLogService
{
    private readonly LogDbContext _context;

    public PerformanceLogService(LogDbContext context)
    {
        _context = context;
    }

    public async Task<PerformanceLogDto> LogPerformanceAsync(CreatePerformanceLogRequest request, CancellationToken cancellationToken = default)
    {
        var performance = new PerformanceLog
        {
            Timestamp = DateTime.UtcNow,
            ServiceName = request.ServiceName,
            EndpointName = request.EndpointName,
            DurationMs = request.DurationMs,
            MemoryUsedMB = request.MemoryUsedMB,
            CpuUsagePercent = request.CpuUsagePercent,
            RequestCount = request.RequestCount,
            SuccessCount = request.SuccessCount,
            ErrorCount = request.ErrorCount,
            HttpMethod = request.HttpMethod,
            StatusCode = request.StatusCode,
            AdditionalData = request.AdditionalData
        };

        _context.PerformanceLogs.Add(performance);
        await _context.SaveChangesAsync(cancellationToken);

        return MapToDto(performance);
    }

    public async Task<PagedResult<PerformanceLogDto>> GetPerformanceLogsAsync(int pageNumber, int pageSize, string? serviceName = null, DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken cancellationToken = default)
    {
        var query = _context.PerformanceLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(serviceName))
            query = query.Where(x => x.ServiceName == serviceName);

        if (dateFrom.HasValue)
            query = query.Where(x => x.Timestamp >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(x => x.Timestamp <= dateTo.Value);

        query = query.OrderByDescending(x => x.Timestamp);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResult<PerformanceLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<LogStatisticsDto> GetServiceStatisticsAsync(string serviceName, DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default)
    {
        var query = _context.PerformanceLogs.Where(x => x.ServiceName == serviceName);

        var start = startDate ?? DateTime.UtcNow.AddDays(-7);
        var end = endDate ?? DateTime.UtcNow;

        query = query.Where(x => x.Timestamp >= start && x.Timestamp <= end);

        var logs = await query.ToListAsync(cancellationToken);

        if (logs.Count == 0)
        {
            return new LogStatisticsDto
            {
                ServiceName = serviceName,
                StartDate = start,
                EndDate = end
            };
        }

        var totalRequests = logs.Sum(x => x.RequestCount);
        var successCount = logs.Sum(x => x.SuccessCount);
        var errorCount = logs.Sum(x => x.ErrorCount);

        return new LogStatisticsDto
        {
            ServiceName = serviceName,
            StartDate = start,
            EndDate = end,
            TotalRequests = totalRequests,
            SuccessCount = successCount,
            ErrorCount = errorCount,
            AverageDurationMs = logs.Average(x => x.DurationMs),
            MinDurationMs = logs.Min(x => x.DurationMs),
            MaxDurationMs = logs.Max(x => x.DurationMs),
            ErrorRate = totalRequests > 0 ? (double)errorCount / totalRequests * 100 : 0
        };
    }

    public async Task<PagedResult<PerformanceLogDto>> GetSlowRequestsAsync(long thresholdMs, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.PerformanceLogs
            .Where(x => x.DurationMs >= thresholdMs)
            .OrderByDescending(x => x.DurationMs);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(x => MapToDto(x))
            .ToListAsync(cancellationToken);

        return new PagedResult<PerformanceLogDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    private static PerformanceLogDto MapToDto(PerformanceLog performance)
    {
        return new PerformanceLogDto
        {
            Id = performance.Id,
            Timestamp = performance.Timestamp,
            ServiceName = performance.ServiceName,
            EndpointName = performance.EndpointName,
            DurationMs = performance.DurationMs,
            MemoryUsedMB = performance.MemoryUsedMB,
            CpuUsagePercent = performance.CpuUsagePercent,
            RequestCount = performance.RequestCount,
            SuccessCount = performance.SuccessCount,
            ErrorCount = performance.ErrorCount,
            HttpMethod = performance.HttpMethod,
            StatusCode = performance.StatusCode,
            AdditionalData = performance.AdditionalData
        };
    }
}
