using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.MasterData.Api.Entities;

/// <summary>
/// Represents a district in Vietnam
/// </summary>
public class District : BaseEntity
{
    /// <summary>
    /// Gets or sets the district code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the district name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the province ID
    /// </summary>
    public Guid ProvinceId { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Navigation property for the parent province
    /// </summary>
    public virtual Province Province { get; set; } = null!;

    /// <summary>
    /// Navigation property for wards in this district
    /// </summary>
    public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();

    /// <summary>
    /// Navigation property for industrial zones in this district
    /// </summary>
    public virtual ICollection<IndustrialZone> IndustrialZones { get; set; } = new List<IndustrialZone>();
}
