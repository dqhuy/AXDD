namespace AXDD.Services.Search.Api.Models;

/// <summary>
/// Enterprise document for Elasticsearch indexing
/// </summary>
public class EnterpriseSearchDocument
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Enterprise name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Tax code (unique identifier)
    /// </summary>
    public string TaxCode { get; set; } = string.Empty;

    /// <summary>
    /// Enterprise address
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Industry code
    /// </summary>
    public string? IndustryCode { get; set; }

    /// <summary>
    /// Industry name
    /// </summary>
    public string? IndustryName { get; set; }

    /// <summary>
    /// Current status (Active, Inactive, etc.)
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Industrial zone name
    /// </summary>
    public string? IndustrialZoneName { get; set; }

    /// <summary>
    /// Industrial zone ID
    /// </summary>
    public int? IndustrialZoneId { get; set; }

    /// <summary>
    /// Contact person name
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// Contact phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Contact email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Registered capital amount
    /// </summary>
    public decimal? RegisteredCapital { get; set; }

    /// <summary>
    /// Date of registration
    /// </summary>
    public DateTime? RegisteredDate { get; set; }

    /// <summary>
    /// Enterprise description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Date when the document was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date when the document was last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
