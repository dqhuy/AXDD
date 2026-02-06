using AXDD.Services.GIS.Api.Data;
using AXDD.Services.GIS.Api.DTOs;
using AXDD.Services.GIS.Api.Entities;
using AXDD.Services.GIS.Api.Exceptions;
using AXDD.Services.GIS.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AXDD.Services.GIS.Api.Services.Implementations;

/// <summary>
/// Implementation of industrial zone service
/// </summary>
public class IndustrialZoneService : IIndustrialZoneService
{
    private readonly GisDbContext _context;
    private readonly ISpatialQueryService _spatialQueryService;
    private readonly ILogger<IndustrialZoneService> _logger;

    public IndustrialZoneService(
        GisDbContext context,
        ISpatialQueryService spatialQueryService,
        ILogger<IndustrialZoneService> logger)
    {
        _context = context;
        _spatialQueryService = spatialQueryService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<IndustrialZoneDto> CreateZoneAsync(
        CreateIndustrialZoneRequest request,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check if code already exists
        var existing = await _context.IndustrialZones
            .FirstOrDefaultAsync(z => z.Code == request.Code, cancellationToken);

        if (existing != null)
        {
            throw new InvalidOperationException($"Industrial zone with code '{request.Code}' already exists");
        }

        // Create polygon from DTO
        var boundary = ConvertPolygonDtoToGeometry(request.Boundary);

        // Calculate centroid and area
        var centroid = _spatialQueryService.GetCentroid(boundary);
        var areaHectares = _spatialQueryService.CalculateAreaHectares(boundary);

        var zone = new IndustrialZone
        {
            Name = request.Name,
            Code = request.Code,
            Boundary = boundary,
            AreaHectares = areaHectares,
            CentroidLatitude = centroid.Y,
            CentroidLongitude = centroid.X,
            Description = request.Description,
            Province = request.Province,
            District = request.District,
            EstablishedYear = request.EstablishedYear,
            Status = IndustrialZoneStatus.Active
        };

        _context.IndustrialZones.Add(zone);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created industrial zone {ZoneCode} with ID {ZoneId}", zone.Code, zone.Id);

        return await GetZoneAsync(zone.Id, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<IndustrialZoneDto> GetZoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var zone = await _context.IndustrialZones
            .Include(z => z.EnterpriseLocations)
            .FirstOrDefaultAsync(z => z.Id == id, cancellationToken);

        if (zone == null)
        {
            throw new IndustrialZoneNotFoundException(id);
        }

        return new IndustrialZoneDto
        {
            Id = zone.Id,
            Name = zone.Name,
            Code = zone.Code,
            Boundary = ConvertGeometryToPolygonDto(zone.Boundary),
            AreaHectares = zone.AreaHectares,
            CentroidLatitude = zone.CentroidLatitude,
            CentroidLongitude = zone.CentroidLongitude,
            Description = zone.Description,
            Province = zone.Province,
            District = zone.District,
            Status = zone.Status.ToString(),
            EstablishedYear = zone.EstablishedYear,
            EnterpriseCount = zone.EnterpriseLocations.Count,
            CreatedAt = zone.CreatedAt
        };
    }

    /// <inheritdoc/>
    public async Task<PolygonDto> GetZoneBoundaryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var zone = await _context.IndustrialZones
            .FirstOrDefaultAsync(z => z.Id == id, cancellationToken);

        if (zone == null)
        {
            throw new IndustrialZoneNotFoundException(id);
        }

        return ConvertGeometryToPolygonDto(zone.Boundary);
    }

    /// <inheritdoc/>
    public async Task<List<IndustrialZoneSummaryDto>> GetZonesAsync(CancellationToken cancellationToken = default)
    {
        var zones = await _context.IndustrialZones
            .Include(z => z.EnterpriseLocations)
            .OrderBy(z => z.Name)
            .ToListAsync(cancellationToken);

        return zones.Select(z => new IndustrialZoneSummaryDto
        {
            Id = z.Id,
            Name = z.Name,
            Code = z.Code,
            AreaHectares = z.AreaHectares,
            CentroidLatitude = z.CentroidLatitude,
            CentroidLongitude = z.CentroidLongitude,
            Province = z.Province,
            Status = z.Status.ToString(),
            EnterpriseCount = z.EnterpriseLocations.Count
        }).ToList();
    }

    /// <inheritdoc/>
    public async Task UpdateZoneBoundaryAsync(
        Guid id,
        UpdateZoneBoundaryRequest request,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var zone = await _context.IndustrialZones
            .FirstOrDefaultAsync(z => z.Id == id, cancellationToken);

        if (zone == null)
        {
            throw new IndustrialZoneNotFoundException(id);
        }

        // Create new polygon
        var newBoundary = ConvertPolygonDtoToGeometry(request.Boundary);

        // Recalculate centroid and area
        var centroid = _spatialQueryService.GetCentroid(newBoundary);
        var areaHectares = _spatialQueryService.CalculateAreaHectares(newBoundary);

        zone.Boundary = newBoundary;
        zone.AreaHectares = areaHectares;
        zone.CentroidLatitude = centroid.Y;
        zone.CentroidLongitude = centroid.X;

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Updated boundary for industrial zone {ZoneId}", id);
    }

    /// <inheritdoc/>
    public async Task<double> CalculateZoneAreaAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var zone = await _context.IndustrialZones
            .FirstOrDefaultAsync(z => z.Id == id, cancellationToken);

        if (zone == null)
        {
            throw new IndustrialZoneNotFoundException(id);
        }

        return _spatialQueryService.CalculateAreaHectares(zone.Boundary);
    }

    /// <inheritdoc/>
    public async Task DeleteZoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var zone = await _context.IndustrialZones
            .FirstOrDefaultAsync(z => z.Id == id, cancellationToken);

        if (zone == null)
        {
            throw new IndustrialZoneNotFoundException(id);
        }

        _context.IndustrialZones.Remove(zone);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted industrial zone {ZoneId}", id);
    }

    /// <inheritdoc/>
    public async Task<List<IndustrialZoneSummaryDto>> SearchZonesAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetZonesAsync(cancellationToken);
        }

        var zones = await _context.IndustrialZones
            .Include(z => z.EnterpriseLocations)
            .Where(z => z.Name.Contains(searchTerm) ||
                       z.Code.Contains(searchTerm) ||
                       (z.Province != null && z.Province.Contains(searchTerm)))
            .OrderBy(z => z.Name)
            .ToListAsync(cancellationToken);

        return zones.Select(z => new IndustrialZoneSummaryDto
        {
            Id = z.Id,
            Name = z.Name,
            Code = z.Code,
            AreaHectares = z.AreaHectares,
            CentroidLatitude = z.CentroidLatitude,
            CentroidLongitude = z.CentroidLongitude,
            Province = z.Province,
            Status = z.Status.ToString(),
            EnterpriseCount = z.EnterpriseLocations.Count
        }).ToList();
    }

    /// <summary>
    /// Convert PolygonDto (GeoJSON) to NetTopologySuite Polygon
    /// </summary>
    private NetTopologySuite.Geometries.Polygon ConvertPolygonDtoToGeometry(PolygonDto polygonDto)
    {
        if (polygonDto?.Coordinates == null || polygonDto.Coordinates.Count == 0)
        {
            throw new ArgumentException("Invalid polygon: no coordinates provided");
        }

        // Get the exterior ring (first array)
        var exteriorRing = polygonDto.Coordinates[0];

        if (exteriorRing.Count < 3)
        {
            throw new ArgumentException("Polygon must have at least 3 points");
        }

        // Convert to array format expected by SpatialQueryService
        var coords = exteriorRing.Select(c => new double[] { c[0], c[1] }).ToArray();

        // Remove the last coordinate if it's a duplicate of the first (GeoJSON closes rings)
        if (coords.Length > 3 &&
            coords[0][0] == coords[^1][0] &&
            coords[0][1] == coords[^1][1])
        {
            coords = coords[..^1];
        }

        return _spatialQueryService.CreatePolygon(coords);
    }

    /// <summary>
    /// Convert NetTopologySuite Polygon to PolygonDto (GeoJSON)
    /// </summary>
    private static PolygonDto ConvertGeometryToPolygonDto(NetTopologySuite.Geometries.Polygon polygon)
    {
        var exteriorRing = polygon.ExteriorRing.Coordinates
            .Select(c => new List<double> { c.X, c.Y })
            .ToList();

        return new PolygonDto
        {
            Type = "Polygon",
            Coordinates = new List<List<List<double>>> { exteriorRing }
        };
    }
}
