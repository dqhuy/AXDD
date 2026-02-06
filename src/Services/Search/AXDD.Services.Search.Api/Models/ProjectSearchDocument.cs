namespace AXDD.Services.Search.Api.Models;

/// <summary>
/// Project search document for Elasticsearch indexing
/// </summary>
public class ProjectSearchDocument
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Project name
    /// </summary>
    public string ProjectName { get; set; } = string.Empty;

    /// <summary>
    /// Project code
    /// </summary>
    public string ProjectCode { get; set; } = string.Empty;

    /// <summary>
    /// Investment amount in VND
    /// </summary>
    public decimal? InvestmentAmount { get; set; }

    /// <summary>
    /// Project status (Planning, InProgress, Completed, etc.)
    /// </summary>
    public string Status { get; set; } = "Planning";

    /// <summary>
    /// Associated enterprise code
    /// </summary>
    public string? EnterpriseCode { get; set; }

    /// <summary>
    /// Associated enterprise name
    /// </summary>
    public string? EnterpriseName { get; set; }

    /// <summary>
    /// Project start date
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// Project end date
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Project description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Project location/address
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Industrial zone name
    /// </summary>
    public string? IndustrialZoneName { get; set; }

    /// <summary>
    /// Industrial zone ID
    /// </summary>
    public int? IndustrialZoneId { get; set; }

    /// <summary>
    /// Project category/type
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Date when the document was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when the document was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
