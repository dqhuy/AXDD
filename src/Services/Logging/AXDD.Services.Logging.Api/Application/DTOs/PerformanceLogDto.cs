namespace AXDD.Services.Logging.Api.Application.DTOs;

/// <summary>
/// DTO for performance log
/// </summary>
public class PerformanceLogDto
{
    public Guid Id { get; set; }
    public DateTime Timestamp { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public string EndpointName { get; set; } = string.Empty;
    public long DurationMs { get; set; }
    public double? MemoryUsedMB { get; set; }
    public double? CpuUsagePercent { get; set; }
    public int RequestCount { get; set; }
    public int SuccessCount { get; set; }
    public int ErrorCount { get; set; }
    public string? HttpMethod { get; set; }
    public int? StatusCode { get; set; }
    public string? AdditionalData { get; set; }
}

/// <summary>
/// Request DTO for creating a performance log
/// </summary>
public class CreatePerformanceLogRequest
{
    public string ServiceName { get; set; } = string.Empty;
    public string EndpointName { get; set; } = string.Empty;
    public long DurationMs { get; set; }
    public double? MemoryUsedMB { get; set; }
    public double? CpuUsagePercent { get; set; }
    public int RequestCount { get; set; } = 1;
    public int SuccessCount { get; set; } = 0;
    public int ErrorCount { get; set; } = 0;
    public string? HttpMethod { get; set; }
    public int? StatusCode { get; set; }
    public string? AdditionalData { get; set; }
}
