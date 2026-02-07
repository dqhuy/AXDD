using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.Enterprise.Api.Domain.Enums;

namespace AXDD.Services.Enterprise.Api.Domain.Entities;

/// <summary>
/// Represents a license or permit held by an enterprise
/// </summary>
public class EnterpriseLicense : BaseEntity
{
    /// <summary>
    /// Gets or sets the enterprise ID
    /// </summary>
    public Guid EnterpriseId { get; set; }

    /// <summary>
    /// Gets or sets the license type
    /// </summary>
    public LicenseType LicenseType { get; set; }

    /// <summary>
    /// Gets or sets the license number
    /// </summary>
    public string LicenseNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the issued date
    /// </summary>
    public DateTime? IssuedDate { get; set; }

    /// <summary>
    /// Gets or sets the expiry date
    /// </summary>
    public DateTime? ExpiryDate { get; set; }

    /// <summary>
    /// Gets or sets the issuing authority
    /// </summary>
    public string? IssuingAuthority { get; set; }

    /// <summary>
    /// Gets or sets the license status
    /// </summary>
    public string Status { get; set; } = "Active";

    /// <summary>
    /// Gets or sets the file ID (link to File Service)
    /// </summary>
    public Guid? FileId { get; set; }

    /// <summary>
    /// Gets or sets the notes
    /// </summary>
    public string? Notes { get; set; }

    // Navigation properties

    /// <summary>
    /// Gets or sets the enterprise
    /// </summary>
    public EnterpriseEntity? Enterprise { get; set; }
}
