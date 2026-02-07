using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.Enterprise.Api.Domain.Enums;

namespace AXDD.Services.Enterprise.Api.Domain.Entities;

/// <summary>
/// Represents an enterprise in the system
/// </summary>
public class EnterpriseEntity : AuditableEntity
{
    /// <summary>
    /// Gets or sets the unique enterprise code (e.g., DN-BH1-001)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the enterprise name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the tax code (10 or 13 digits)
    /// </summary>
    public string TaxCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the English name (optional)
    /// </summary>
    public string? EnglishName { get; set; }

    /// <summary>
    /// Gets or sets the short name
    /// </summary>
    public string? ShortName { get; set; }

    /// <summary>
    /// Gets or sets the industry code (VSIC)
    /// </summary>
    public string IndustryCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the industry name
    /// </summary>
    public string IndustryName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the industrial zone ID
    /// </summary>
    public Guid? IndustrialZoneId { get; set; }

    /// <summary>
    /// Gets or sets the industrial zone name
    /// </summary>
    public string? IndustrialZoneName { get; set; }

    /// <summary>
    /// Gets or sets the enterprise status
    /// </summary>
    public EnterpriseStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the legal representative name
    /// </summary>
    public string? LegalRepresentative { get; set; }

    /// <summary>
    /// Gets or sets the position of legal representative
    /// </summary>
    public string? Position { get; set; }

    /// <summary>
    /// Gets or sets the address
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ward
    /// </summary>
    public string? Ward { get; set; }

    /// <summary>
    /// Gets or sets the district
    /// </summary>
    public string? District { get; set; }

    /// <summary>
    /// Gets or sets the province
    /// </summary>
    public string? Province { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the fax number
    /// </summary>
    public string? Fax { get; set; }

    /// <summary>
    /// Gets or sets the email address
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the website URL
    /// </summary>
    public string? Website { get; set; }

    /// <summary>
    /// Gets or sets the registration date
    /// </summary>
    public DateTime? RegisteredDate { get; set; }

    /// <summary>
    /// Gets or sets the registered capital in VND
    /// </summary>
    public decimal? RegisteredCapital { get; set; }

    /// <summary>
    /// Gets or sets the charter capital in VND
    /// </summary>
    public decimal? CharterCapital { get; set; }

    /// <summary>
    /// Gets or sets the total number of employees
    /// </summary>
    public int? TotalEmployees { get; set; }

    /// <summary>
    /// Gets or sets the number of Vietnamese employees
    /// </summary>
    public int? VietnamEmployees { get; set; }

    /// <summary>
    /// Gets or sets the number of foreign employees
    /// </summary>
    public int? ForeignEmployees { get; set; }

    /// <summary>
    /// Gets or sets the production capacity description
    /// </summary>
    public string? ProductionCapacity { get; set; }

    /// <summary>
    /// Gets or sets the annual revenue in VND
    /// </summary>
    public decimal? AnnualRevenue { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string? Description { get; set; }

    // Navigation properties

    /// <summary>
    /// Gets or sets the collection of contact persons
    /// </summary>
    public ICollection<ContactPerson> Contacts { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of licenses
    /// </summary>
    public ICollection<EnterpriseLicense> Licenses { get; set; } = [];

    /// <summary>
    /// Gets or sets the collection of history records
    /// </summary>
    public ICollection<EnterpriseHistory> History { get; set; } = [];
}
