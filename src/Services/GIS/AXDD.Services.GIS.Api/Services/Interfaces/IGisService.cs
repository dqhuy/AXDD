using AXDD.Services.GIS.Api.DTOs;
using AXDD.Services.GIS.Api.Entities;

namespace AXDD.Services.GIS.Api.Services.Interfaces;

/// <summary>
/// Service for managing geographic locations of enterprises
/// </summary>
public interface IGisService
{
    /// <summary>
    /// Save or update an enterprise location
    /// </summary>
    /// <param name="enterpriseCode">Enterprise code</param>
    /// <param name="request">Location data</param>
    /// <param name="userId">User ID performing the operation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Enterprise location DTO</returns>
    Task<EnterpriseLocationDto> SaveEnterpriseLocationAsync(
        string enterpriseCode,
        SaveEnterpriseLocationRequest request,
        string? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get enterprise location by code
    /// </summary>
    /// <param name="enterpriseCode">Enterprise code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Enterprise location DTO or null if not found</returns>
    Task<EnterpriseLocationDto?> GetEnterpriseLocationAsync(
        string enterpriseCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get enterprises within a radius from a point
    /// </summary>
    /// <param name="latitude">Center point latitude</param>
    /// <param name="longitude">Center point longitude</param>
    /// <param name="radiusKm">Radius in kilometers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of enterprises with distances</returns>
    Task<List<EnterpriseLocationDto>> GetEnterprisesByProximityAsync(
        double latitude,
        double longitude,
        double radiusKm,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all enterprises within an industrial zone
    /// </summary>
    /// <param name="industrialZoneId">Industrial zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of enterprise locations</returns>
    Task<List<EnterpriseLocationDto>> GetEnterprisesInZoneAsync(
        Guid industrialZoneId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a point is within any industrial zone
    /// </summary>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Point in zone result with zone information if found</returns>
    Task<PointInZoneResult> IsPointInZoneAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete enterprise location
    /// </summary>
    /// <param name="enterpriseCode">Enterprise code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteEnterpriseLocationAsync(
        string enterpriseCode,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all enterprise locations (paginated)
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of enterprise locations</returns>
    Task<(List<EnterpriseLocationDto> Items, int TotalCount)> GetAllEnterpriseLocationsAsync(
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}
