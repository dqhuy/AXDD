using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.MasterData.Api.Entities;

/// <summary>
/// Represents a ward/commune in Vietnam
/// </summary>
public class Ward : BaseEntity
{
    /// <summary>
    /// Gets or sets the ward code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the ward name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the district ID
    /// </summary>
    public Guid DistrictId { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Navigation property for the parent district
    /// </summary>
    public virtual District District { get; set; } = null!;
}
