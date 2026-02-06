using AXDD.Services.GIS.Api.Exceptions;
using AXDD.Services.GIS.Api.Services.Interfaces;
using AXDD.Services.GIS.Api.Settings;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

namespace AXDD.Services.GIS.Api.Services.Implementations;

/// <summary>
/// Implementation of spatial query service
/// </summary>
public class SpatialQueryService : ISpatialQueryService
{
    private readonly GeometryFactory _geometryFactory;
    private readonly GisSettings _gisSettings;

    public SpatialQueryService(IOptions<GisSettings> gisSettings)
    {
        _gisSettings = gisSettings.Value;
        _geometryFactory = new GeometryFactory(new PrecisionModel(), _gisSettings.DefaultSRID);
    }

    /// <inheritdoc/>
    public bool PointInPolygon(Point point, Polygon polygon)
    {
        ArgumentNullException.ThrowIfNull(point);
        ArgumentNullException.ThrowIfNull(polygon);

        return polygon.Contains(point);
    }

    /// <inheritdoc/>
    public double DistanceBetween(Point point1, Point point2)
    {
        ArgumentNullException.ThrowIfNull(point1);
        ArgumentNullException.ThrowIfNull(point2);

        // Use Haversine formula for accurate distance calculation
        return CalculateHaversineDistance(
            point1.Y, point1.X,
            point2.Y, point2.X
        );
    }

    /// <inheritdoc/>
    public Polygon BufferAroundPoint(Point point, double radiusKm)
    {
        ArgumentNullException.ThrowIfNull(point);

        if (radiusKm <= 0)
        {
            throw new ArgumentException("Radius must be positive", nameof(radiusKm));
        }

        // Convert kilometers to degrees (approximate)
        // At equator: 1 degree ≈ 111 km
        // This is an approximation; for more accuracy, use proper projection
        var radiusDegrees = radiusKm / 111.0;

        // Create a buffer using NetTopologySuite
        var buffer = point.Buffer(radiusDegrees);

        if (buffer is Polygon polygon)
        {
            return polygon;
        }

        throw new SpatialQueryException("Failed to create buffer polygon");
    }

    /// <inheritdoc/>
    public bool Intersects(Polygon polygon1, Polygon polygon2)
    {
        ArgumentNullException.ThrowIfNull(polygon1);
        ArgumentNullException.ThrowIfNull(polygon2);

        return polygon1.Intersects(polygon2);
    }

    /// <inheritdoc/>
    public double CalculateAreaHectares(Polygon polygon)
    {
        ArgumentNullException.ThrowIfNull(polygon);

        // Get area in square degrees
        var areaDegrees = polygon.Area;

        // Convert to square meters (approximate)
        // This is a rough approximation; for accuracy, project to UTM
        var lat = polygon.Centroid.Y;
        var metersPerDegreeLat = 111320.0; // meters per degree latitude
        var metersPerDegreeLon = 111320.0 * Math.Cos(lat * Math.PI / 180.0); // meters per degree longitude

        var areaSquareMeters = areaDegrees * metersPerDegreeLat * metersPerDegreeLon;

        // Convert to hectares (1 hectare = 10,000 square meters)
        return areaSquareMeters / 10000.0;
    }

    /// <inheritdoc/>
    public Point GetCentroid(Polygon polygon)
    {
        ArgumentNullException.ThrowIfNull(polygon);
        return polygon.Centroid;
    }

    /// <inheritdoc/>
    public Point CreatePoint(double latitude, double longitude, int srid = 4326)
    {
        if (!ValidateCoordinates(latitude, longitude))
        {
            throw new InvalidCoordinatesException(latitude, longitude);
        }

        var factory = new GeometryFactory(new PrecisionModel(), srid);
        // Note: NetTopologySuite uses (X, Y) = (Longitude, Latitude)
        return factory.CreatePoint(new Coordinate(longitude, latitude));
    }

    /// <inheritdoc/>
    public Polygon CreatePolygon(double[][] coordinates, int srid = 4326)
    {
        ArgumentNullException.ThrowIfNull(coordinates);

        if (coordinates.Length < 3)
        {
            throw new ArgumentException("Polygon must have at least 3 points", nameof(coordinates));
        }

        var factory = new GeometryFactory(new PrecisionModel(), srid);
        var coords = new Coordinate[coordinates.Length + 1]; // +1 to close the ring

        for (int i = 0; i < coordinates.Length; i++)
        {
            if (coordinates[i].Length != 2)
            {
                throw new ArgumentException($"Coordinate at index {i} must have exactly 2 values [lon, lat]");
            }

            var lon = coordinates[i][0];
            var lat = coordinates[i][1];

            if (!ValidateCoordinates(lat, lon))
            {
                throw new InvalidCoordinatesException($"Invalid coordinates at index {i}: lat={lat}, lon={lon}");
            }

            coords[i] = new Coordinate(lon, lat);
        }

        // Close the ring by repeating the first coordinate
        coords[coordinates.Length] = coords[0];

        var shell = factory.CreateLinearRing(coords);
        return factory.CreatePolygon(shell);
    }

    /// <inheritdoc/>
    public bool ValidateCoordinates(double latitude, double longitude)
    {
        return latitude >= -90 && latitude <= 90 &&
               longitude >= -180 && longitude <= 180;
    }

    /// <inheritdoc/>
    public bool ValidateVietnamBounds(double latitude, double longitude)
    {
        if (!ValidateCoordinates(latitude, longitude))
        {
            return false;
        }

        return _gisSettings.VietnamBounds.IsWithinBounds(latitude, longitude);
    }

    /// <summary>
    /// Calculate distance between two points using Haversine formula
    /// </summary>
    private static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}
