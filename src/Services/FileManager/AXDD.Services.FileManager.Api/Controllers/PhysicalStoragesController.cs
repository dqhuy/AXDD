using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for physical storage operations
/// </summary>
[ApiController]
[Route("api/v1/physical-storages")]
[Produces("application/json")]
public class PhysicalStoragesController : ControllerBase
{
    private readonly IPhysicalStorageService _physicalStorageService;
    private readonly ILogger<PhysicalStoragesController> _logger;

    public PhysicalStoragesController(IPhysicalStorageService physicalStorageService, ILogger<PhysicalStoragesController> logger)
    {
        _physicalStorageService = physicalStorageService;
        _logger = logger;
    }

    /// <summary>
    /// Lists all physical storages with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<PhysicalStorageDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] string? enterpriseCode = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _physicalStorageService.ListAsync(enterpriseCode, pageNumber, pageSize, isActive, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<PhysicalStorageDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets a physical storage by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PhysicalStorageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, [FromQuery] bool includeLocations = false, CancellationToken cancellationToken = default)
    {
        var result = await _physicalStorageService.GetByIdAsync(id, includeLocations, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PhysicalStorageDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets a physical storage by code
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(ApiResponse<PhysicalStorageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken = default)
    {
        var result = await _physicalStorageService.GetByCodeAsync(code, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PhysicalStorageDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Creates a new physical storage
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PhysicalStorageDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePhysicalStorageRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Code is required"));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Name is required"));
        }

        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _physicalStorageService.CreateAsync(request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id },
            ApiResponse<PhysicalStorageDto>.SuccessResponse(result.Value, "Physical storage created successfully"));
    }

    /// <summary>
    /// Updates a physical storage
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<PhysicalStorageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreatePhysicalStorageRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _physicalStorageService.UpdateAsync(id, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PhysicalStorageDto>.SuccessResponse(result.Value!, "Physical storage updated successfully"));
    }

    /// <summary>
    /// Deletes a physical storage
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _physicalStorageService.DeleteAsync(id, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }

    /// <summary>
    /// Lists locations in a physical storage
    /// </summary>
    [HttpGet("{storageId}/locations")]
    [ProducesResponseType(typeof(ApiResponse<List<PhysicalStorageLocationDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListLocations(Guid storageId, CancellationToken cancellationToken = default)
    {
        var result = await _physicalStorageService.ListLocationsAsync(storageId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<List<PhysicalStorageLocationDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Adds a location to a physical storage
    /// </summary>
    [HttpPost("{storageId}/locations")]
    [ProducesResponseType(typeof(ApiResponse<PhysicalStorageLocationDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddLocation(Guid storageId, [FromBody] CreatePhysicalStorageLocationRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _physicalStorageService.AddLocationAsync(storageId, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Created("", ApiResponse<PhysicalStorageLocationDto>.SuccessResponse(result.Value!, "Location added successfully"));
    }

    /// <summary>
    /// Updates a location in a physical storage
    /// </summary>
    [HttpPut("{storageId}/locations/{locationId}")]
    [ProducesResponseType(typeof(ApiResponse<PhysicalStorageLocationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateLocation(Guid storageId, Guid locationId, [FromBody] CreatePhysicalStorageLocationRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _physicalStorageService.UpdateLocationAsync(storageId, locationId, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PhysicalStorageLocationDto>.SuccessResponse(result.Value!, "Location updated successfully"));
    }

    /// <summary>
    /// Deletes a location from a physical storage
    /// </summary>
    [HttpDelete("{storageId}/locations/{locationId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLocation(Guid storageId, Guid locationId, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _physicalStorageService.DeleteLocationAsync(storageId, locationId, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }
}
