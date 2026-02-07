namespace AXDD.Services.Search.Api.DTOs;

/// <summary>
/// Base search request with common search parameters
/// </summary>
public class SearchRequest
{
    /// <summary>
    /// Search query text
    /// </summary>
    public string Query { get; set; } = string.Empty;

    /// <summary>
    /// Page number (1-based)
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of results per page
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Field to sort by
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Sort direction (asc or desc)
    /// </summary>
    public string SortDirection { get; set; } = "desc";

    /// <summary>
    /// Enable fuzzy search for typo tolerance
    /// </summary>
    public bool EnableFuzzy { get; set; } = true;
}

/// <summary>
/// Enterprise search request with specific filters
/// </summary>
public class EnterpriseSearchRequest : SearchRequest
{
    /// <summary>
    /// Filters for enterprise search
    /// </summary>
    public EnterpriseFilters Filters { get; set; } = new();
}

/// <summary>
/// Filters for enterprise search
/// </summary>
public class EnterpriseFilters
{
    /// <summary>
    /// Filter by status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by industrial zone ID
    /// </summary>
    public int? IndustrialZoneId { get; set; }

    /// <summary>
    /// Filter by industry code
    /// </summary>
    public string? IndustryCode { get; set; }

    /// <summary>
    /// Filter by registered date from
    /// </summary>
    public DateTime? RegisteredDateFrom { get; set; }

    /// <summary>
    /// Filter by registered date to
    /// </summary>
    public DateTime? RegisteredDateTo { get; set; }

    /// <summary>
    /// Filter by minimum registered capital
    /// </summary>
    public decimal? MinRegisteredCapital { get; set; }

    /// <summary>
    /// Filter by maximum registered capital
    /// </summary>
    public decimal? MaxRegisteredCapital { get; set; }
}

/// <summary>
/// Document search request with specific filters
/// </summary>
public class DocumentSearchRequest : SearchRequest
{
    /// <summary>
    /// Filters for document search
    /// </summary>
    public DocumentFilters Filters { get; set; } = new();
}

/// <summary>
/// Filters for document search
/// </summary>
public class DocumentFilters
{
    /// <summary>
    /// Filter by file type
    /// </summary>
    public string? FileType { get; set; }

    /// <summary>
    /// Filter by enterprise code
    /// </summary>
    public string? EnterpriseCode { get; set; }

    /// <summary>
    /// Filter by category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Filter by upload date from
    /// </summary>
    public DateTime? DateFrom { get; set; }

    /// <summary>
    /// Filter by upload date to
    /// </summary>
    public DateTime? DateTo { get; set; }

    /// <summary>
    /// Filter by tags
    /// </summary>
    public List<string>? Tags { get; set; }

    /// <summary>
    /// Filter by status
    /// </summary>
    public string? Status { get; set; }
}

/// <summary>
/// Project search request with specific filters
/// </summary>
public class ProjectSearchRequest : SearchRequest
{
    /// <summary>
    /// Filters for project search
    /// </summary>
    public ProjectFilters Filters { get; set; } = new();
}

/// <summary>
/// Filters for project search
/// </summary>
public class ProjectFilters
{
    /// <summary>
    /// Filter by status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Filter by enterprise code
    /// </summary>
    public string? EnterpriseCode { get; set; }

    /// <summary>
    /// Filter by industrial zone ID
    /// </summary>
    public int? IndustrialZoneId { get; set; }

    /// <summary>
    /// Filter by category
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Filter by start date from
    /// </summary>
    public DateTime? StartDateFrom { get; set; }

    /// <summary>
    /// Filter by start date to
    /// </summary>
    public DateTime? StartDateTo { get; set; }

    /// <summary>
    /// Filter by minimum investment amount
    /// </summary>
    public decimal? MinInvestmentAmount { get; set; }

    /// <summary>
    /// Filter by maximum investment amount
    /// </summary>
    public decimal? MaxInvestmentAmount { get; set; }
}
