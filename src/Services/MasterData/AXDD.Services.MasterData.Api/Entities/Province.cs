using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.MasterData.Api.Entities;

/// <summary>
/// Represents a province in Vietnam
/// </summary>
public class Province : BaseEntity
{
    /// <summary>
    /// Gets or sets the province code (e.g., "89" for Dong Nai)
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the province name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the region (North, Central, South)
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Navigation property for districts in this province
    /// </summary>
    public virtual ICollection<District> Districts { get; set; } = new List<District>();

    /// <summary>
    /// Navigation property for industrial zones in this province
    /// </summary>
    public virtual ICollection<IndustrialZone> IndustrialZones { get; set; } = new List<IndustrialZone>();
}
