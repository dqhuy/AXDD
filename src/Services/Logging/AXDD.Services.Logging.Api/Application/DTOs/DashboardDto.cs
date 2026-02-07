namespace AXDD.Services.Logging.Api.Application.DTOs;

/// <summary>
/// DTO for log statistics
/// </summary>
public class LogStatisticsDto
{
    public string ServiceName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalRequests { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public double AverageDurationMs { get; set; }
    public long MinDurationMs { get; set; }
    public long MaxDurationMs { get; set; }
    public double ErrorRate { get; set; }
}

/// <summary>
/// DTO for dashboard summary
/// </summary>
public class DashboardSummaryDto
{
    public int TotalLogsToday { get; set; }
    public int ErrorsToday { get; set; }
    public int ActiveUsersToday { get; set; }
    public double AverageResponseTimeMs { get; set; }
    public int CriticalErrorsUnresolved { get; set; }
    public List<ServiceLogCount> LogsByService { get; set; } = new();
    public List<HourlyLogCount> LogsByHour { get; set; } = new();
    public List<TopUserActivity> TopUsers { get; set; } = new();
    public List<SlowEndpoint> SlowestEndpoints { get; set; } = new();
}

/// <summary>
/// Service log count
/// </summary>
public class ServiceLogCount
{
    public string ServiceName { get; set; } = string.Empty;
    public int LogCount { get; set; }
    public int ErrorCount { get; set; }
}

/// <summary>
/// Hourly log count
/// </summary>
public class HourlyLogCount
{
    public int Hour { get; set; }
    public int LogCount { get; set; }
}

/// <summary>
/// Top user activity
/// </summary>
public class TopUserActivity
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public int ActivityCount { get; set; }
}

/// <summary>
/// Slow endpoint
/// </summary>
public class SlowEndpoint
{
    public string ServiceName { get; set; } = string.Empty;
    public string EndpointName { get; set; } = string.Empty;
    public double AverageDurationMs { get; set; }
    public int RequestCount { get; set; }
}
