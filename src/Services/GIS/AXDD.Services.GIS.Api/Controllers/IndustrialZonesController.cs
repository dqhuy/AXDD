using AXDD.Services.GIS.Api.DTOs;
using AXDD.Services.GIS.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.GIS.Api.Controllers;

/// <summary>
/// Controller for industrial zone management
/// </summary>
[ApiController]
[Route("api/v1/gis/industrial-zones")]
[Produces("application/json")]
public class IndustrialZonesController : ControllerBase
{
    private readonly IIndustrialZoneService _industrialZoneService;
    private readonly IGisService _gisService;
    private readonly ILogger<IndustrialZonesController> _logger;

    public IndustrialZonesController(
        IIndustrialZoneService industrialZoneService,
        IGisService gisService,
        ILogger<IndustrialZonesController> logger)
    {
        _industrialZoneService = industrialZoneService;
        _gisService = gisService;
        _logger = logger;
    }

    /// <summary>
    /// Get all industrial zones
    /// </summary>
    /// <param name="search">Optional search term</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of industrial zones</returns>
    /// <response code="200">Zones retrieved successfully</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<IndustrialZoneSummaryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<IndustrialZoneSummaryDto>>> GetZonesAsync(
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        List<IndustrialZoneSummaryDto> zones;

        if (!string.IsNullOrWhiteSpace(search))
        {
            zones = await _industrialZoneService.SearchZonesAsync(search, cancellationToken);
        }
        else
        {
            zones = await _industrialZoneService.GetZonesAsync(cancellationToken);
        }

        return Ok(zones);
    }

    /// <summary>
    /// Get industrial zone by ID
    /// </summary>
    /// <param name="id">Zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Industrial zone details</returns>
    /// <response code="200">Zone found</response>
    /// <response code="404">Zone not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IndustrialZoneDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IndustrialZoneDto>> GetZoneAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var zone = await _industrialZoneService.GetZoneAsync(id, cancellationToken);
        return Ok(zone);
    }

    /// <summary>
    /// Get industrial zone boundary in GeoJSON format
    /// </summary>
    /// <param name="id">Zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Zone boundary as GeoJSON polygon</returns>
    /// <response code="200">Boundary retrieved successfully</response>
    /// <response code="404">Zone not found</response>
    [HttpGet("{id}/boundary")]
    [ProducesResponseType(typeof(PolygonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PolygonDto>> GetZoneBoundaryAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var boundary = await _industrialZoneService.GetZoneBoundaryAsync(id, cancellationToken);
        return Ok(boundary);
    }

    /// <summary>
    /// Get enterprises within an industrial zone
    /// </summary>
    /// <param name="id">Zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of enterprises in the zone</returns>
    /// <response code="200">Enterprises retrieved successfully</response>
    /// <response code="404">Zone not found</response>
    [HttpGet("{id}/enterprises")]
    [ProducesResponseType(typeof(List<EnterpriseLocationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<EnterpriseLocationDto>>> GetEnterprisesInZoneAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var enterprises = await _gisService.GetEnterprisesInZoneAsync(id, cancellationToken);
        return Ok(enterprises);
    }

    /// <summary>
    /// Create a new industrial zone
    /// </summary>
    /// <param name="request">Zone creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created industrial zone</returns>
    /// <response code="201">Zone created successfully</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost]
    [ProducesResponseType(typeof(IndustrialZoneDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IndustrialZoneDto>> CreateZoneAsync(
        [FromBody] CreateIndustrialZoneRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name;
        var zone = await _industrialZoneService.CreateZoneAsync(request, userId, cancellationToken);

        return CreatedAtAction(
            nameof(GetZoneAsync),
            new { id = zone.Id },
            zone);
    }

    /// <summary>
    /// Update industrial zone boundary
    /// </summary>
    /// <param name="id">Zone ID</param>
    /// <param name="request">Boundary update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Boundary updated successfully</response>
    /// <response code="400">Invalid request data</response>
    /// <response code="404">Zone not found</response>
    [HttpPut("{id}/boundary")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateZoneBoundaryAsync(
        Guid id,
        [FromBody] UpdateZoneBoundaryRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name;
        await _industrialZoneService.UpdateZoneBoundaryAsync(id, request, userId, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Calculate industrial zone area
    /// </summary>
    /// <param name="id">Zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Area in hectares</returns>
    /// <response code="200">Area calculated successfully</response>
    /// <response code="404">Zone not found</response>
    [HttpGet("{id}/area")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> CalculateZoneAreaAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var area = await _industrialZoneService.CalculateZoneAreaAsync(id, cancellationToken);
        return Ok(new { AreaHectares = area });
    }

    /// <summary>
    /// Delete an industrial zone
    /// </summary>
    /// <param name="id">Zone ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Zone deleted successfully</response>
    /// <response code="404">Zone not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteZoneAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await _industrialZoneService.DeleteZoneAsync(id, cancellationToken);
        return NoContent();
    }
}
