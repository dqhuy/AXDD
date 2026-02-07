using AXDD.Services.GIS.Api.DTOs;
using AXDD.Services.GIS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.GIS.Api.Controllers;

/// <summary>
/// Controller for GIS operations (enterprise locations)
/// </summary>
[ApiController]
[Route("api/v1/gis")]
[Produces("application/json")]
public class GisController : ControllerBase
{
    private readonly IGisService _gisService;
    private readonly ILogger<GisController> _logger;

    public GisController(IGisService gisService, ILogger<GisController> logger)
    {
        _gisService = gisService;
        _logger = logger;
    }

    /// <summary>
    /// Save or update an enterprise location
    /// </summary>
    /// <param name="enterpriseCode">Enterprise code</param>
    /// <param name="request">Location data</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Enterprise location</returns>
    /// <response code="200">Location saved successfully</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost("enterprises/{enterpriseCode}/location")]
    [ProducesResponseType(typeof(EnterpriseLocationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EnterpriseLocationDto>> SaveEnterpriseLocationAsync(
        string enterpriseCode,
        [FromBody] SaveEnterpriseLocationRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name;
        var result = await _gisService.SaveEnterpriseLocationAsync(
            enterpriseCode,
            request,
            userId,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Get enterprise location by code
    /// </summary>
    /// <param name="enterpriseCode">Enterprise code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Enterprise location</returns>
    /// <response code="200">Location found</response>
    /// <response code="404">Location not found</response>
    [HttpGet("enterprises/{enterpriseCode}/location")]
    [ProducesResponseType(typeof(EnterpriseLocationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EnterpriseLocationDto>> GetEnterpriseLocationAsync(
        string enterpriseCode,
        CancellationToken cancellationToken = default)
    {
        var result = await _gisService.GetEnterpriseLocationAsync(enterpriseCode, cancellationToken);

        if (result == null)
        {
            return NotFound(new { Message = $"Location not found for enterprise {enterpriseCode}" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get enterprises near a location
    /// </summary>
    /// <param name="latitude">Center point latitude</param>
    /// <param name="longitude">Center point longitude</param>
    /// <param name="radiusKm">Search radius in kilometers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of nearby enterprises with distances</returns>
    /// <response code="200">Enterprises found</response>
    /// <response code="400">Invalid coordinates or radius</response>
    [HttpGet("enterprises/nearby")]
    [ProducesResponseType(typeof(List<EnterpriseLocationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<EnterpriseLocationDto>>> GetEnterprisesByProximityAsync(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radiusKm = 10.0,
        CancellationToken cancellationToken = default)
    {
        var result = await _gisService.GetEnterprisesByProximityAsync(
            latitude,
            longitude,
            radiusKm,
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Check if a point is within an industrial zone
    /// </summary>
    /// <param name="latitude">Latitude</param>
    /// <param name="longitude">Longitude</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Point in zone result with zone information if found</returns>
    /// <response code="200">Query completed</response>
    /// <response code="400">Invalid coordinates</response>
    [HttpGet("point-in-zone")]
    [ProducesResponseType(typeof(PointInZoneResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PointInZoneResult>> IsPointInZoneAsync(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        CancellationToken cancellationToken = default)
    {
        var result = await _gisService.IsPointInZoneAsync(latitude, longitude, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Delete enterprise location
    /// </summary>
    /// <param name="enterpriseCode">Enterprise code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Location deleted successfully</response>
    /// <response code="404">Location not found</response>
    [HttpDelete("enterprises/{enterpriseCode}/location")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteEnterpriseLocationAsync(
        string enterpriseCode,
        CancellationToken cancellationToken = default)
    {
        await _gisService.DeleteEnterpriseLocationAsync(enterpriseCode, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Get all enterprise locations (paginated)
    /// </summary>
    /// <param name="pageNumber">Page number (1-based)</param>
    /// <param name="pageSize">Page size (max 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of enterprise locations</returns>
    /// <response code="200">Locations retrieved successfully</response>
    [HttpGet("enterprises/locations")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAllEnterpriseLocationsAsync(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await _gisService.GetAllEnterpriseLocationsAsync(
            pageNumber,
            pageSize,
            cancellationToken);

        return Ok(new
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }
}
