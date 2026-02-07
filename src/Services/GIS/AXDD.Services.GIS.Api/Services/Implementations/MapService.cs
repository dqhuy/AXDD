using AXDD.Services.GIS.Api.Services.Interfaces;
using AXDD.Services.GIS.Api.Settings;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

namespace AXDD.Services.GIS.Api.Services.Implementations;

/// <summary>
/// Implementation of map service
/// </summary>
public class MapService : IMapService
{
    private readonly MapSettings _mapSettings;

    public MapService(IOptions<MapSettings> mapSettings)
    {
        _mapSettings = mapSettings.Value;
    }

    /// <inheritdoc/>
    public string GetMapTileUrl(int zoom, int x, int y)
    {
        if (zoom < 0 || zoom > 19)
        {
            throw new ArgumentException("Zoom level must be between 0 and 19", nameof(zoom));
        }

        return _mapSettings.TileServerUrl
            .Replace("{z}", zoom.ToString())
            .Replace("{x}", x.ToString())
            .Replace("{y}", y.ToString());
    }

    /// <inheritdoc/>
    public string GenerateStaticMapUrl(
        double latitude,
        double longitude,
        int zoom,
        List<(double lat, double lon)>? markers = null,
        int? width = null,
        int? height = null)
    {
        var w = width ?? _mapSettings.DefaultWidth;
        var h = height ?? _mapSettings.DefaultHeight;

        // Generate OpenStreetMap embed URL
        var bbox = CalculateBoundingBox(latitude, longitude, zoom, w, h);

        var url = $"{_mapSettings.StaticMapUrl}?bbox={bbox.minLon},{bbox.minLat},{bbox.maxLon},{bbox.maxLat}";
        url += $"&layer=mapnik&marker={latitude},{longitude}";

        return url;
    }

    /// <inheritdoc/>
    public (double minLat, double minLon, double maxLat, double maxLon) GetBoundingBox(Polygon polygon)
    {
        ArgumentNullException.ThrowIfNull(polygon);

        var envelope = polygon.EnvelopeInternal;

        return (
            minLat: envelope.MinY,
            minLon: envelope.MinX,
            maxLat: envelope.MaxY,
            maxLon: envelope.MaxX
        );
    }

    /// <inheritdoc/>
    public int CalculateZoomLevel(
        double minLat,
        double minLon,
        double maxLat,
        double maxLon,
        int mapWidth = 600,
        int mapHeight = 450)
    {
        // Calculate the span in degrees
        var latSpan = Math.Abs(maxLat - minLat);
        var lonSpan = Math.Abs(maxLon - minLon);

        // Calculate zoom level based on span
        // This is an approximation for Web Mercator projection
        var latZoom = CalculateZoomForSpan(latSpan, mapHeight);
        var lonZoom = CalculateZoomForSpan(lonSpan, mapWidth);

        // Use the minimum zoom to ensure everything fits
        var zoom = Math.Min(latZoom, lonZoom);

        // Clamp to valid range
        return Math.Max(0, Math.Min(19, zoom));
    }

    /// <summary>
    /// Calculate bounding box for a center point and zoom level
    /// </summary>
    private static (double minLat, double minLon, double maxLat, double maxLon) CalculateBoundingBox(
        double centerLat,
        double centerLon,
        int zoom,
        int width,
        int height)
    {
        // Calculate degrees per pixel
        var degreesPerPixel = 360.0 / (256 * Math.Pow(2, zoom));

        // Calculate half-width and half-height in degrees
        var halfWidthDegrees = (width / 2.0) * degreesPerPixel;
        var halfHeightDegrees = (height / 2.0) * degreesPerPixel;

        return (
            minLat: centerLat - halfHeightDegrees,
            minLon: centerLon - halfWidthDegrees,
            maxLat: centerLat + halfHeightDegrees,
            maxLon: centerLon + halfWidthDegrees
        );
    }

    /// <summary>
    /// Calculate zoom level for a given span in degrees
    /// </summary>
    private static int CalculateZoomForSpan(double spanDegrees, int pixelSize)
    {
        if (spanDegrees <= 0)
        {
            return 19; // Maximum zoom
        }

        // Calculate zoom level
        // zoom = log2(360 * pixelSize / (spanDegrees * 256))
        var zoom = Math.Log(360.0 * pixelSize / (spanDegrees * 256), 2);

        return (int)Math.Floor(zoom);
    }
}
