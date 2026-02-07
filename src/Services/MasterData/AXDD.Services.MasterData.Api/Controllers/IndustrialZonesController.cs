using AXDD.Services.MasterData.Api.DTOs;
using AXDD.Services.MasterData.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.MasterData.Api.Controllers;

/// <summary>
/// Controller for managing industrial zones
/// </summary>
[ApiController]
[Route("api/v1/masterdata/[controller]")]
[Produces("application/json")]
public class IndustrialZonesController : ControllerBase
{
    private readonly IIndustrialZoneService _service;
    private readonly ILogger<IndustrialZonesController> _logger;

    public IndustrialZonesController(
        IIndustrialZoneService service,
        ILogger<IndustrialZonesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets all industrial zones
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<IndustrialZoneDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<IndustrialZoneDto>>> GetAll(
        [FromQuery] Guid? provinceId = null,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        var zones = await _service.GetAllAsync(provinceId, status, cancellationToken);
        return Ok(zones);
    }

    /// <summary>
    /// Gets an industrial zone by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(IndustrialZoneDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IndustrialZoneDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var zone = await _service.GetByIdAsync(id, cancellationToken);
        if (zone == null)
        {
            return NotFound();
        }

        return Ok(zone);
    }

    /// <summary>
    /// Gets an industrial zone by code
    /// </summary>
    [HttpGet("by-code/{code}")]
    [ProducesResponseType(typeof(IndustrialZoneDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IndustrialZoneDto>> GetByCode(string code, CancellationToken cancellationToken)
    {
        var zone = await _service.GetByCodeAsync(code, cancellationToken);
        if (zone == null)
        {
            return NotFound();
        }

        return Ok(zone);
    }

    /// <summary>
    /// Creates a new industrial zone (Admin only)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(IndustrialZoneDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IndustrialZoneDto>> Create(
        [FromBody] CreateIndustrialZoneRequest request,
        CancellationToken cancellationToken)
    {
        var userId = "system"; // TODO: Get from authentication context
        var zone = await _service.CreateAsync(request, userId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = zone.Id }, zone);
    }

    /// <summary>
    /// Updates an existing industrial zone (Admin only)
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(IndustrialZoneDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IndustrialZoneDto>> Update(
        Guid id,
        [FromBody] UpdateIndustrialZoneRequest request,
        CancellationToken cancellationToken)
    {
        var userId = "system"; // TODO: Get from authentication context
        var zone = await _service.UpdateAsync(id, request, userId, cancellationToken);
        return Ok(zone);
    }
}
