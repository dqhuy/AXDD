using AXDD.BuildingBlocks.Domain.Entities;
using NetTopologySuite.Geometries;

namespace AXDD.Services.GIS.Api.Entities;

/// <summary>
/// Represents an industrial zone with geographic boundaries
/// </summary>
public class IndustrialZone : BaseEntity
{
    /// <summary>
    /// Name of the industrial zone
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Unique code for the industrial zone
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Geographic boundary polygon (NetTopologySuite Polygon with SRID 4326)
    /// </summary>
    public Polygon Boundary { get; set; } = null!;

    /// <summary>
    /// Calculated area in hectares
    /// </summary>
    public double AreaHectares { get; set; }

    /// <summary>
    /// Centroid latitude for display and quick reference
    /// </summary>
    public double CentroidLatitude { get; set; }

    /// <summary>
    /// Centroid longitude for display and quick reference
    /// </summary>
    public double CentroidLongitude { get; set; }

    /// <summary>
    /// Description of the industrial zone
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Province or city where the zone is located
    /// </summary>
    public string? Province { get; set; }

    /// <summary>
    /// District where the zone is located
    /// </summary>
    public string? District { get; set; }

    /// <summary>
    /// Status of the industrial zone
    /// </summary>
    public IndustrialZoneStatus Status { get; set; } = IndustrialZoneStatus.Active;

    /// <summary>
    /// Year the zone was established
    /// </summary>
    public int? EstablishedYear { get; set; }

    /// <summary>
    /// Collection of enterprise locations within this zone
    /// </summary>
    public ICollection<EnterpriseLocation> EnterpriseLocations { get; set; } = new List<EnterpriseLocation>();

    /// <summary>
    /// Collection of land plots within this zone
    /// </summary>
    public ICollection<LandPlot> LandPlots { get; set; } = new List<LandPlot>();
}

/// <summary>
/// Status of an industrial zone
/// </summary>
public enum IndustrialZoneStatus
{
    /// <summary>
    /// Zone is currently active and operational
    /// </summary>
    Active = 1,

    /// <summary>
    /// Zone is planned but not yet operational
    /// </summary>
    Planned = 2,

    /// <summary>
    /// Zone is under construction
    /// </summary>
    UnderConstruction = 3,

    /// <summary>
    /// Zone is temporarily inactive
    /// </summary>
    Inactive = 4,

    /// <summary>
    /// Zone has been closed
    /// </summary>
    Closed = 5
}
