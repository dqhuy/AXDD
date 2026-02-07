using AXDD.BuildingBlocks.Domain.Entities;
using NetTopologySuite.Geometries;

namespace AXDD.Services.GIS.Api.Entities;

/// <summary>
/// Represents a land plot within an industrial zone
/// </summary>
public class LandPlot : BaseEntity
{
    /// <summary>
    /// Plot number or identifier
    /// </summary>
    public string PlotNumber { get; set; } = string.Empty;

    /// <summary>
    /// Industrial zone this plot belongs to
    /// </summary>
    public Guid IndustrialZoneId { get; set; }

    /// <summary>
    /// Navigation property to industrial zone
    /// </summary>
    public IndustrialZone IndustrialZone { get; set; } = null!;

    /// <summary>
    /// Geographic boundary of the plot (Polygon)
    /// </summary>
    public Polygon Geometry { get; set; } = null!;

    /// <summary>
    /// Area of the plot in square meters
    /// </summary>
    public double AreaSquareMeters { get; set; }

    /// <summary>
    /// Owner enterprise ID (if allocated)
    /// </summary>
    public Guid? OwnerEnterpriseId { get; set; }

    /// <summary>
    /// Owner enterprise code
    /// </summary>
    public string? OwnerEnterpriseCode { get; set; }

    /// <summary>
    /// Status of the land plot
    /// </summary>
    public LandPlotStatus Status { get; set; } = LandPlotStatus.Available;

    /// <summary>
    /// Lease start date (if leased)
    /// </summary>
    public DateTime? LeaseStartDate { get; set; }

    /// <summary>
    /// Lease end date (if leased)
    /// </summary>
    public DateTime? LeaseEndDate { get; set; }

    /// <summary>
    /// Price per square meter
    /// </summary>
    public decimal? PricePerSquareMeter { get; set; }

    /// <summary>
    /// Notes about the plot
    /// </summary>
    public string? Notes { get; set; }
}

/// <summary>
/// Status of a land plot
/// </summary>
public enum LandPlotStatus
{
    /// <summary>
    /// Plot is available for lease
    /// </summary>
    Available = 1,

    /// <summary>
    /// Plot is reserved but not yet leased
    /// </summary>
    Reserved = 2,

    /// <summary>
    /// Plot is currently leased
    /// </summary>
    Leased = 3,

    /// <summary>
    /// Plot is under development
    /// </summary>
    UnderDevelopment = 4,

    /// <summary>
    /// Plot is not available
    /// </summary>
    Unavailable = 5
}
