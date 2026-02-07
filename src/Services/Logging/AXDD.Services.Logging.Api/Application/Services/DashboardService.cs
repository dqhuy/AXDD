using AXDD.Services.Logging.Api.Application.DTOs;
using AXDD.Services.Logging.Api.Application.Services.Interfaces;
using AXDD.Services.Logging.Api.Domain.Enums;
using AXDD.Services.Logging.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.Logging.Api.Application.Services;

/// <summary>
/// Service for dashboard operations
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly LogDbContext _context;

    public DashboardService(LogDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        // Total logs today
        var totalLogsToday = await _context.AuditLogs
            .Where(x => x.Timestamp >= today && x.Timestamp < tomorrow)
            .CountAsync(cancellationToken);

        // Errors today
        var errorsToday = await _context.ErrorLogs
            .Where(x => x.Timestamp >= today && x.Timestamp < tomorrow)
            .CountAsync(cancellationToken);

        // Active users today
        var activeUsersToday = await _context.UserActivityLogs
            .Where(x => x.Timestamp >= today && x.Timestamp < tomorrow)
            .Select(x => x.UserId)
            .Distinct()
            .CountAsync(cancellationToken);

        // Average response time
        var performanceLogsToday = await _context.PerformanceLogs
            .Where(x => x.Timestamp >= today && x.Timestamp < tomorrow)
            .ToListAsync(cancellationToken);

        var averageResponseTime = performanceLogsToday.Any()
            ? performanceLogsToday.Average(x => x.DurationMs)
            : 0;

        // Critical errors unresolved
        var criticalErrorsUnresolved = await _context.ErrorLogs
            .Where(x => x.Severity == ErrorSeverity.Critical && !x.IsResolved)
            .CountAsync(cancellationToken);

        // Logs by service (today)
        var logsByService = await _context.AuditLogs
            .Where(x => x.Timestamp >= today && x.Timestamp < tomorrow)
            .GroupBy(x => x.ServiceName)
            .Select(g => new ServiceLogCount
            {
                ServiceName = g.Key,
                LogCount = g.Count(),
                ErrorCount = g.Count(x => x.Level == AuditLogLevel.Error || x.Level == AuditLogLevel.Critical)
            })
            .OrderByDescending(x => x.LogCount)
            .Take(10)
            .ToListAsync(cancellationToken);

        // Logs by hour (last 24 hours)
        var last24Hours = DateTime.UtcNow.AddHours(-24);
        var logsByHour = await _context.AuditLogs
            .Where(x => x.Timestamp >= last24Hours)
            .GroupBy(x => x.Timestamp.Hour)
            .Select(g => new HourlyLogCount
            {
                Hour = g.Key,
                LogCount = g.Count()
            })
            .OrderBy(x => x.Hour)
            .ToListAsync(cancellationToken);

        // Top users (today)
        var topUsers = await _context.UserActivityLogs
            .Where(x => x.Timestamp >= today && x.Timestamp < tomorrow)
            .GroupBy(x => new { x.UserId, x.Username })
            .Select(g => new TopUserActivity
            {
                UserId = g.Key.UserId,
                Username = g.Key.Username,
                ActivityCount = g.Count()
            })
            .OrderByDescending(x => x.ActivityCount)
            .Take(10)
            .ToListAsync(cancellationToken);

        // Slowest endpoints (today)
        var slowestEndpoints = await _context.PerformanceLogs
            .Where(x => x.Timestamp >= today && x.Timestamp < tomorrow)
            .GroupBy(x => new { x.ServiceName, x.EndpointName })
            .Select(g => new SlowEndpoint
            {
                ServiceName = g.Key.ServiceName,
                EndpointName = g.Key.EndpointName,
                AverageDurationMs = g.Average(x => x.DurationMs),
                RequestCount = g.Sum(x => x.RequestCount)
            })
            .OrderByDescending(x => x.AverageDurationMs)
            .Take(10)
            .ToListAsync(cancellationToken);

        return new DashboardSummaryDto
        {
            TotalLogsToday = totalLogsToday,
            ErrorsToday = errorsToday,
            ActiveUsersToday = activeUsersToday,
            AverageResponseTimeMs = averageResponseTime,
            CriticalErrorsUnresolved = criticalErrorsUnresolved,
            LogsByService = logsByService,
            LogsByHour = logsByHour,
            TopUsers = topUsers,
            SlowestEndpoints = slowestEndpoints
        };
    }
}
