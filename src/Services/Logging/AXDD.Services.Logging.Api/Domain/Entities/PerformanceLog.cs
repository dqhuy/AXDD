using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.Logging.Api.Domain.Entities;

/// <summary>
/// Represents a performance log entry for tracking system performance metrics
/// </summary>
public class PerformanceLog : BaseEntity
{
    /// <summary>
    /// Gets or sets the timestamp
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the service name
    /// </summary>
    public string ServiceName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the endpoint name
    /// </summary>
    public string EndpointName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the duration in milliseconds
    /// </summary>
    public long DurationMs { get; set; }

    /// <summary>
    /// Gets or sets the memory used in megabytes
    /// </summary>
    public double? MemoryUsedMB { get; set; }

    /// <summary>
    /// Gets or sets the CPU usage percentage
    /// </summary>
    public double? CpuUsagePercent { get; set; }

    /// <summary>
    /// Gets or sets the request count
    /// </summary>
    public int RequestCount { get; set; } = 1;

    /// <summary>
    /// Gets or sets the success count
    /// </summary>
    public int SuccessCount { get; set; }

    /// <summary>
    /// Gets or sets the error count
    /// </summary>
    public int ErrorCount { get; set; }

    /// <summary>
    /// Gets or sets the HTTP method
    /// </summary>
    public string? HttpMethod { get; set; }

    /// <summary>
    /// Gets or sets the HTTP status code
    /// </summary>
    public int? StatusCode { get; set; }

    /// <summary>
    /// Gets or sets additional metadata as JSON
    /// </summary>
    public string? AdditionalData { get; set; }
}
