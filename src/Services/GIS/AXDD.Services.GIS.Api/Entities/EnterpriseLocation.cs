using AXDD.BuildingBlocks.Domain.Entities;
using NetTopologySuite.Geometries;

namespace AXDD.Services.GIS.Api.Entities;

/// <summary>
/// Represents a geographic location for an enterprise
/// </summary>
public class EnterpriseLocation : BaseEntity
{
    /// <summary>
    /// Enterprise identifier from the Enterprise service
    /// </summary>
    public Guid EnterpriseId { get; set; }

    /// <summary>
    /// Enterprise code for easy lookup
    /// </summary>
    public string EnterpriseCode { get; set; } = string.Empty;

    /// <summary>
    /// Enterprise name (cached for display)
    /// </summary>
    public string EnterpriseName { get; set; } = string.Empty;

    /// <summary>
    /// Geographic point location (NetTopologySuite Point with SRID 4326)
    /// </summary>
    public Point Location { get; set; } = null!;

    /// <summary>
    /// Latitude in decimal degrees (WGS84)
    /// Stored separately for easy display and queries
    /// </summary>
    public double Latitude { get; set; }

    /// <summary>
    /// Longitude in decimal degrees (WGS84)
    /// Stored separately for easy display and queries
    /// </summary>
    public double Longitude { get; set; }

    /// <summary>
    /// Physical address of the location
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// GPS accuracy in meters
    /// </summary>
    public double? Accuracy { get; set; }

    /// <summary>
    /// Notes or additional information about the location
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Whether this is the primary location for the enterprise
    /// </summary>
    public bool IsPrimary { get; set; } = true;

    /// <summary>
    /// Industrial zone this location belongs to (if any)
    /// </summary>
    public Guid? IndustrialZoneId { get; set; }

    /// <summary>
    /// Navigation property to industrial zone
    /// </summary>
    public IndustrialZone? IndustrialZone { get; set; }
}
