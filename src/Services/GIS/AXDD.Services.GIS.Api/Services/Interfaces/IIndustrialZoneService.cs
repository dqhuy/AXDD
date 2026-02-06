using AXDD.Services.GIS.Api.DTOs;

namespace AXDD.Services.GIS.Api.Services.Interfaces;

/// <summary>
/// Service for managing industrial zones
/// </summary>
public interface IIndustrialZoneService
{
    /// <summary>
    /// Create a new industrial zone
    /// </summary>
    /// <param name="request">Zone creation request</param>
    /// <param name="userId">User ID performing the operation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created industrial zone DTO</returns>
    Task<IndustrialZoneDto> CreateZoneAsync(
        CreateIndustrialZoneRequest request,
        string? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get industrial zone by ID
    /// </summary>
    /// <param name="id">Zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Industrial zone DTO</returns>
    Task<IndustrialZoneDto> GetZoneAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get industrial zone boundary in GeoJSON format
    /// </summary>
    /// <param name="id">Zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Polygon DTO in GeoJSON format</returns>
    Task<PolygonDto> GetZoneBoundaryAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all industrial zones
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of industrial zone summaries</returns>
    Task<List<IndustrialZoneSummaryDto>> GetZonesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Update industrial zone boundary
    /// </summary>
    /// <param name="id">Zone ID</param>
    /// <param name="request">Boundary update request</param>
    /// <param name="userId">User ID performing the operation</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateZoneBoundaryAsync(
        Guid id,
        UpdateZoneBoundaryRequest request,
        string? userId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculate the area of an industrial zone in hectares
    /// </summary>
    /// <param name="id">Zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Area in hectares</returns>
    Task<double> CalculateZoneAreaAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete an industrial zone
    /// </summary>
    /// <param name="id">Zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task DeleteZoneAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Search industrial zones by name or code
    /// </summary>
    /// <param name="searchTerm">Search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching zones</returns>
    Task<List<IndustrialZoneSummaryDto>> SearchZonesAsync(
        string searchTerm,
        CancellationToken cancellationToken = default);
}
