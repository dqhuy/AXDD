using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.MasterData.Api.Entities;

/// <summary>
/// Represents an industrial zone in Vietnam
/// </summary>
public class IndustrialZone : BaseEntity
{
    /// <summary>
    /// Gets or sets the zone code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the zone name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the province ID
    /// </summary>
    public Guid ProvinceId { get; set; }

    /// <summary>
    /// Gets or sets the district ID
    /// </summary>
    public Guid? DistrictId { get; set; }

    /// <summary>
    /// Gets or sets the area in hectares
    /// </summary>
    public decimal Area { get; set; }

    /// <summary>
    /// Gets or sets the status (Active, UnderConstruction, Planned, etc.)
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the established date
    /// </summary>
    public DateTime? EstablishedDate { get; set; }

    /// <summary>
    /// Gets or sets the management unit
    /// </summary>
    public string? ManagementUnit { get; set; }

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Navigation property for the province
    /// </summary>
    public virtual Province Province { get; set; } = null!;

    /// <summary>
    /// Navigation property for the district
    /// </summary>
    public virtual District? District { get; set; }
}
