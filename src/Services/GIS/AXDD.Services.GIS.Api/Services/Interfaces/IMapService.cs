namespace AXDD.Services.GIS.Api.Services.Interfaces;

/// <summary>
/// Service for map-related operations
/// </summary>
public interface IMapService
{
    /// <summary>
    /// Get OpenStreetMap tile URL
    /// </summary>
    /// <param name="zoom">Zoom level (0-19)</param>
    /// <param name="x">Tile X coordinate</param>
    /// <param name="y">Tile Y coordinate</param>
    /// <returns>Tile URL</returns>
    string GetMapTileUrl(int zoom, int x, int y);

    /// <summary>
    /// Generate a static map URL with markers
    /// </summary>
    /// <param name="latitude">Center latitude</param>
    /// <param name="longitude">Center longitude</param>
    /// <param name="zoom">Zoom level</param>
    /// <param name="markers">Optional markers (lat,lon pairs)</param>
    /// <param name="width">Map width in pixels</param>
    /// <param name="height">Map height in pixels</param>
    /// <returns>Static map URL</returns>
    string GenerateStaticMapUrl(
        double latitude,
        double longitude,
        int zoom,
        List<(double lat, double lon)>? markers = null,
        int? width = null,
        int? height = null);

    /// <summary>
    /// Get bounding box for a polygon
    /// </summary>
    /// <param name="polygon">Polygon geometry</param>
    /// <returns>Bounding box (minLat, minLon, maxLat, maxLon)</returns>
    (double minLat, double minLon, double maxLat, double maxLon) GetBoundingBox(NetTopologySuite.Geometries.Polygon polygon);

    /// <summary>
    /// Calculate appropriate zoom level for a bounding box
    /// </summary>
    /// <param name="minLat">Minimum latitude</param>
    /// <param name="minLon">Minimum longitude</param>
    /// <param name="maxLat">Maximum latitude</param>
    /// <param name="maxLon">Maximum longitude</param>
    /// <param name="mapWidth">Map width in pixels</param>
    /// <param name="mapHeight">Map height in pixels</param>
    /// <returns>Appropriate zoom level</returns>
    int CalculateZoomLevel(
        double minLat,
        double minLon,
        double maxLat,
        double maxLon,
        int mapWidth = 600,
        int mapHeight = 450);
}
