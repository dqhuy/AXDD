using AXDD.Services.Logging.Api.Domain.Enums;

namespace AXDD.Services.Logging.Api.Application.DTOs;

/// <summary>
/// DTO for filtering logs
/// </summary>
public class LogFilterDto
{
    /// <summary>
    /// Filter by start date
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    /// Filter by end date
    /// </summary>
    public DateTime? DateTo { get; set; }

    /// <summary>
    /// Filter by log level
    /// </summary>
    public AuditLogLevel? Level { get; set; }

    /// <summary>
    /// Filter by service name
    /// </summary>
    public string? ServiceName { get; set; }

    /// <summary>
    /// Filter by user ID
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Filter by correlation ID
    /// </summary>
    public string? CorrelationId { get; set; }

    /// <summary>
    /// Search term for full-text search
    /// </summary>
    public string? SearchTerm { get; set; }

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Page size
    /// </summary>
    public int PageSize { get; set; } = 50;

    /// <summary>
    /// Sort by field (e.g., "Timestamp", "Level")
    /// </summary>
    public string SortBy { get; set; } = "Timestamp";

    /// <summary>
    /// Sort direction (asc, desc)
    /// </summary>
    public string SortDirection { get; set; } = "desc";
}

/// <summary>
/// Paginated response
/// </summary>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
