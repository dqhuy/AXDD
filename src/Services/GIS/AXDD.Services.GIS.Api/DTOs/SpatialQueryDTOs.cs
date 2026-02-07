namespace AXDD.Services.GIS.Api.DTOs;

/// <summary>
/// Spatial query request
/// </summary>
public class SpatialQueryRequest
{
    /// <summary>
    /// Type of spatial query
    /// </summary>
    public SpatialQueryType Type { get; set; }

    /// <summary>
    /// First point (for distance, buffer, point-in-polygon queries)
    /// </summary>
    public PointDto? Point1 { get; set; }

    /// <summary>
    /// Second point (for distance queries)
    /// </summary>
    public PointDto? Point2 { get; set; }

    /// <summary>
    /// Polygon (for point-in-polygon, intersection queries)
    /// </summary>
    public PolygonDto? Polygon1 { get; set; }

    /// <summary>
    /// Second polygon (for intersection queries)
    /// </summary>
    public PolygonDto? Polygon2 { get; set; }

    /// <summary>
    /// Radius in kilometers (for buffer queries)
    /// </summary>
    public double? RadiusKm { get; set; }
}

/// <summary>
/// Spatial query response
/// </summary>
public class SpatialQueryResponse
{
    /// <summary>
    /// Query type
    /// </summary>
    public string QueryType { get; set; } = string.Empty;

    /// <summary>
    /// Result value (distance in km, area in hectares, etc.)
    /// </summary>
    public double? Value { get; set; }

    /// <summary>
    /// Boolean result (for intersection, point-in-polygon queries)
    /// </summary>
    public bool? BooleanResult { get; set; }

    /// <summary>
    /// Result polygon (for buffer queries)
    /// </summary>
    public PolygonDto? ResultPolygon { get; set; }

    /// <summary>
    /// Result message
    /// </summary>
    public string? Message { get; set; }
}

/// <summary>
/// Types of spatial queries
/// </summary>
public enum SpatialQueryType
{
    /// <summary>
    /// Calculate distance between two points
    /// </summary>
    DistanceBetweenPoints = 1,

    /// <summary>
    /// Check if a point is within a polygon
    /// </summary>
    PointInPolygon = 2,

    /// <summary>
    /// Create a buffer around a point
    /// </summary>
    BufferAroundPoint = 3,

    /// <summary>
    /// Check if two polygons intersect
    /// </summary>
    PolygonIntersection = 4
}

/// <summary>
/// Result of checking if a point is in an industrial zone
/// </summary>
public class PointInZoneResult
{
    /// <summary>
    /// Whether the point is in a zone
    /// </summary>
    public bool IsInZone { get; set; }

    /// <summary>
    /// Industrial zone information (if found)
    /// </summary>
    public IndustrialZoneSummaryDto? Zone { get; set; }
}
