using AXDD.Services.GIS.Api.Data;
using AXDD.Services.GIS.Api.DTOs;
using AXDD.Services.GIS.Api.Entities;
using AXDD.Services.GIS.Api.Exceptions;
using AXDD.Services.GIS.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace AXDD.Services.GIS.Api.Services.Implementations;

/// <summary>
/// Implementation of GIS service for enterprise locations
/// </summary>
public class GisService : IGisService
{
    private readonly GisDbContext _context;
    private readonly ISpatialQueryService _spatialQueryService;
    private readonly ILogger<GisService> _logger;

    public GisService(
        GisDbContext context,
        ISpatialQueryService spatialQueryService,
        ILogger<GisService> logger)
    {
        _context = context;
        _spatialQueryService = spatialQueryService;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<EnterpriseLocationDto> SaveEnterpriseLocationAsync(
        string enterpriseCode,
        SaveEnterpriseLocationRequest request,
        string? userId = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode);
        ArgumentNullException.ThrowIfNull(request);

        // Validate coordinates
        if (!_spatialQueryService.ValidateCoordinates(request.Latitude, request.Longitude))
        {
            throw new InvalidCoordinatesException(request.Latitude, request.Longitude);
        }

        if (!_spatialQueryService.ValidateVietnamBounds(request.Latitude, request.Longitude))
        {
            _logger.LogWarning(
                "Coordinates ({Latitude}, {Longitude}) are outside Vietnam bounds for enterprise {EnterpriseCode}",
                request.Latitude, request.Longitude, enterpriseCode);
        }

        // Create point geometry
        var location = _spatialQueryService.CreatePoint(request.Latitude, request.Longitude);

        // Check if location already exists
        var existingLocation = await _context.EnterpriseLocations
            .FirstOrDefaultAsync(l => l.EnterpriseCode == enterpriseCode, cancellationToken);

        // Check which industrial zone this point belongs to (if any)
        var industrialZone = await _context.IndustrialZones
            .Where(z => z.Boundary.Contains(location))
            .FirstOrDefaultAsync(cancellationToken);

        if (existingLocation != null)
        {
            // Update existing location
            existingLocation.Location = location;
            existingLocation.Latitude = request.Latitude;
            existingLocation.Longitude = request.Longitude;
            existingLocation.Address = request.Address;
            existingLocation.Accuracy = request.Accuracy;
            existingLocation.Notes = request.Notes;
            existingLocation.IsPrimary = request.IsPrimary;
            existingLocation.IndustrialZoneId = industrialZone?.Id;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Updated location for enterprise {EnterpriseCode} at ({Latitude}, {Longitude})",
                enterpriseCode, request.Latitude, request.Longitude);
        }
        else
        {
            // Create new location
            var newLocation = new EnterpriseLocation
            {
                EnterpriseId = Guid.NewGuid(), // This should come from Enterprise service
                EnterpriseCode = enterpriseCode,
                EnterpriseName = enterpriseCode, // This should come from Enterprise service
                Location = location,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Address = request.Address,
                Accuracy = request.Accuracy,
                Notes = request.Notes,
                IsPrimary = request.IsPrimary,
                IndustrialZoneId = industrialZone?.Id
            };

            _context.EnterpriseLocations.Add(newLocation);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Created location for enterprise {EnterpriseCode} at ({Latitude}, {Longitude})",
                enterpriseCode, request.Latitude, request.Longitude);

            existingLocation = newLocation;
        }

        return await GetEnterpriseLocationAsync(enterpriseCode, cancellationToken)
               ?? throw LocationNotFoundException.ForEnterpriseCode(enterpriseCode);
    }

    /// <inheritdoc/>
    public async Task<EnterpriseLocationDto?> GetEnterpriseLocationAsync(
        string enterpriseCode,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode);

        var location = await _context.EnterpriseLocations
            .Include(l => l.IndustrialZone)
            .FirstOrDefaultAsync(l => l.EnterpriseCode == enterpriseCode, cancellationToken);

        if (location == null)
        {
            return null;
        }

        return new EnterpriseLocationDto
        {
            Id = location.Id,
            EnterpriseCode = location.EnterpriseCode,
            EnterpriseName = location.EnterpriseName,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            Address = location.Address,
            IndustrialZoneName = location.IndustrialZone?.Name,
            IndustrialZoneId = location.IndustrialZoneId,
            IsPrimary = location.IsPrimary,
            CreatedAt = location.CreatedAt,
            UpdatedAt = location.UpdatedAt
        };
    }

    /// <inheritdoc/>
    public async Task<List<EnterpriseLocationDto>> GetEnterprisesByProximityAsync(
        double latitude,
        double longitude,
        double radiusKm,
        CancellationToken cancellationToken = default)
    {
        if (!_spatialQueryService.ValidateCoordinates(latitude, longitude))
        {
            throw new InvalidCoordinatesException(latitude, longitude);
        }

        if (radiusKm <= 0)
        {
            throw new ArgumentException("Radius must be positive", nameof(radiusKm));
        }

        // Create center point
        var centerPoint = _spatialQueryService.CreatePoint(latitude, longitude);

        // Create buffer for spatial query
        var buffer = _spatialQueryService.BufferAroundPoint(centerPoint, radiusKm);

        // Query locations within buffer
        var locations = await _context.EnterpriseLocations
            .Include(l => l.IndustrialZone)
            .Where(l => buffer.Contains(l.Location))
            .ToListAsync(cancellationToken);

        // Calculate distances and create DTOs
        var results = locations.Select(l =>
        {
            var distance = _spatialQueryService.DistanceBetween(centerPoint, l.Location);

            return new EnterpriseLocationDto
            {
                Id = l.Id,
                EnterpriseCode = l.EnterpriseCode,
                EnterpriseName = l.EnterpriseName,
                Latitude = l.Latitude,
                Longitude = l.Longitude,
                Address = l.Address,
                DistanceKm = Math.Round(distance, 2),
                IndustrialZoneName = l.IndustrialZone?.Name,
                IndustrialZoneId = l.IndustrialZoneId,
                IsPrimary = l.IsPrimary,
                CreatedAt = l.CreatedAt,
                UpdatedAt = l.UpdatedAt
            };
        })
        .OrderBy(l => l.DistanceKm)
        .ToList();

        _logger.LogInformation(
            "Found {Count} enterprises within {Radius}km of ({Latitude}, {Longitude})",
            results.Count, radiusKm, latitude, longitude);

        return results;
    }

    /// <inheritdoc/>
    public async Task<List<EnterpriseLocationDto>> GetEnterprisesInZoneAsync(
        Guid industrialZoneId,
        CancellationToken cancellationToken = default)
    {
        var zone = await _context.IndustrialZones
            .FirstOrDefaultAsync(z => z.Id == industrialZoneId, cancellationToken);

        if (zone == null)
        {
            throw new IndustrialZoneNotFoundException(industrialZoneId);
        }

        var locations = await _context.EnterpriseLocations
            .Include(l => l.IndustrialZone)
            .Where(l => l.IndustrialZoneId == industrialZoneId)
            .ToListAsync(cancellationToken);

        return locations.Select(l => new EnterpriseLocationDto
        {
            Id = l.Id,
            EnterpriseCode = l.EnterpriseCode,
            EnterpriseName = l.EnterpriseName,
            Latitude = l.Latitude,
            Longitude = l.Longitude,
            Address = l.Address,
            IndustrialZoneName = l.IndustrialZone?.Name,
            IndustrialZoneId = l.IndustrialZoneId,
            IsPrimary = l.IsPrimary,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt
        }).ToList();
    }

    /// <inheritdoc/>
    public async Task<PointInZoneResult> IsPointInZoneAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        if (!_spatialQueryService.ValidateCoordinates(latitude, longitude))
        {
            throw new InvalidCoordinatesException(latitude, longitude);
        }

        var point = _spatialQueryService.CreatePoint(latitude, longitude);

        var zone = await _context.IndustrialZones
            .Include(z => z.EnterpriseLocations)
            .Where(z => z.Boundary.Contains(point))
            .FirstOrDefaultAsync(cancellationToken);

        if (zone == null)
        {
            return new PointInZoneResult { IsInZone = false };
        }

        return new PointInZoneResult
        {
            IsInZone = true,
            Zone = new IndustrialZoneSummaryDto
            {
                Id = zone.Id,
                Name = zone.Name,
                Code = zone.Code,
                AreaHectares = zone.AreaHectares,
                CentroidLatitude = zone.CentroidLatitude,
                CentroidLongitude = zone.CentroidLongitude,
                Province = zone.Province,
                Status = zone.Status.ToString(),
                EnterpriseCount = zone.EnterpriseLocations.Count
            }
        };
    }

    /// <inheritdoc/>
    public async Task DeleteEnterpriseLocationAsync(
        string enterpriseCode,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(enterpriseCode);

        var location = await _context.EnterpriseLocations
            .FirstOrDefaultAsync(l => l.EnterpriseCode == enterpriseCode, cancellationToken);

        if (location == null)
        {
            throw LocationNotFoundException.ForEnterpriseCode(enterpriseCode);
        }

        _context.EnterpriseLocations.Remove(location);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted location for enterprise {EnterpriseCode}", enterpriseCode);
    }

    /// <inheritdoc/>
    public async Task<(List<EnterpriseLocationDto> Items, int TotalCount)> GetAllEnterpriseLocationsAsync(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        if (pageNumber < 1)
        {
            throw new ArgumentException("Page number must be >= 1", nameof(pageNumber));
        }

        if (pageSize < 1 || pageSize > 100)
        {
            throw new ArgumentException("Page size must be between 1 and 100", nameof(pageSize));
        }

        var query = _context.EnterpriseLocations
            .Include(l => l.IndustrialZone);

        var totalCount = await query.CountAsync(cancellationToken);

        var locations = await query
            .OrderBy(l => l.EnterpriseName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = locations.Select(l => new EnterpriseLocationDto
        {
            Id = l.Id,
            EnterpriseCode = l.EnterpriseCode,
            EnterpriseName = l.EnterpriseName,
            Latitude = l.Latitude,
            Longitude = l.Longitude,
            Address = l.Address,
            IndustrialZoneName = l.IndustrialZone?.Name,
            IndustrialZoneId = l.IndustrialZoneId,
            IsPrimary = l.IsPrimary,
            CreatedAt = l.CreatedAt,
            UpdatedAt = l.UpdatedAt
        }).ToList();

        return (items, totalCount);
    }
}
