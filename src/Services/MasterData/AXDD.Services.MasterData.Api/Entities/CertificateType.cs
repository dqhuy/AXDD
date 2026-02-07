using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.MasterData.Api.Entities;

/// <summary>
/// Represents a type of certificate required for enterprises
/// </summary>
public class CertificateType : BaseEntity
{
    /// <summary>
    /// Gets or sets the certificate type code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the certificate type name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the validity period in months
    /// </summary>
    public int? ValidityPeriod { get; set; }

    /// <summary>
    /// Gets or sets the requiring authority
    /// </summary>
    public string? RequiringAuthority { get; set; }

    /// <summary>
    /// Gets or sets whether this certificate is required
    /// </summary>
    public bool IsRequired { get; set; }

    /// <summary>
    /// Gets or sets whether this certificate type is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}
