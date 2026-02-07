using NetTopologySuite.Geometries;

namespace AXDD.Services.GIS.Api.Services.Interfaces;

/// <summary>
/// Service for spatial query operations
/// </summary>
public interface ISpatialQueryService
{
    /// <summary>
    /// Check if a point is within a polygon
    /// </summary>
    /// <param name="point">Point to check</param>
    /// <param name="polygon">Polygon boundary</param>
    /// <returns>True if point is within polygon</returns>
    bool PointInPolygon(Point point, Polygon polygon);

    /// <summary>
    /// Calculate distance between two points in kilometers
    /// Uses Haversine formula for accuracy
    /// </summary>
    /// <param name="point1">First point</param>
    /// <param name="point2">Second point</param>
    /// <returns>Distance in kilometers</returns>
    double DistanceBetween(Point point1, Point point2);

    /// <summary>
    /// Create a buffer (circle) around a point
    /// </summary>
    /// <param name="point">Center point</param>
    /// <param name="radiusKm">Radius in kilometers</param>
    /// <returns>Polygon representing the buffer</returns>
    Polygon BufferAroundPoint(Point point, double radiusKm);

    /// <summary>
    /// Check if two polygons intersect
    /// </summary>
    /// <param name="polygon1">First polygon</param>
    /// <param name="polygon2">Second polygon</param>
    /// <returns>True if polygons intersect</returns>
    bool Intersects(Polygon polygon1, Polygon polygon2);

    /// <summary>
    /// Calculate the area of a polygon in hectares
    /// </summary>
    /// <param name="polygon">Polygon to measure</param>
    /// <returns>Area in hectares</returns>
    double CalculateAreaHectares(Polygon polygon);

    /// <summary>
    /// Get the centroid (center point) of a polygon
    /// </summary>
    /// <param name="polygon">Polygon</param>
    /// <returns>Centroid point</returns>
    Point GetCentroid(Polygon polygon);

    /// <summary>
    /// Create a Point from latitude and longitude
    /// </summary>
    /// <param name="latitude">Latitude in decimal degrees</param>
    /// <param name="longitude">Longitude in decimal degrees</param>
    /// <param name="srid">Spatial Reference System Identifier (default 4326 for WGS84)</param>
    /// <returns>Point geometry</returns>
    Point CreatePoint(double latitude, double longitude, int srid = 4326);

    /// <summary>
    /// Create a Polygon from coordinate array
    /// </summary>
    /// <param name="coordinates">Array of [longitude, latitude] pairs</param>
    /// <param name="srid">Spatial Reference System Identifier (default 4326 for WGS84)</param>
    /// <returns>Polygon geometry</returns>
    Polygon CreatePolygon(double[][] coordinates, int srid = 4326);

    /// <summary>
    /// Validate if coordinates are within valid ranges
    /// </summary>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    /// <returns>True if valid</returns>
    bool ValidateCoordinates(double latitude, double longitude);

    /// <summary>
    /// Validate if coordinates are within Vietnam bounds
    /// </summary>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    /// <returns>True if within Vietnam</returns>
    bool ValidateVietnamBounds(double latitude, double longitude);
}
