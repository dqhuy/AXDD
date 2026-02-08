using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for digital storage operations
/// </summary>
[ApiController]
[Route("api/v1/digital-storages")]
[Produces("application/json")]
public class DigitalStoragesController : ControllerBase
{
    private readonly IDigitalStorageService _digitalStorageService;
    private readonly ILogger<DigitalStoragesController> _logger;

    public DigitalStoragesController(IDigitalStorageService digitalStorageService, ILogger<DigitalStoragesController> logger)
    {
        _digitalStorageService = digitalStorageService;
        _logger = logger;
    }

    /// <summary>
    /// Lists all digital storages with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DigitalStorageDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] string? enterpriseCode = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _digitalStorageService.ListAsync(enterpriseCode, pageNumber, pageSize, isActive, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<DigitalStorageDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets a digital storage by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<DigitalStorageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _digitalStorageService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DigitalStorageDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets a digital storage by code
    /// </summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(ApiResponse<DigitalStorageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code, CancellationToken cancellationToken = default)
    {
        var result = await _digitalStorageService.GetByCodeAsync(code, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DigitalStorageDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Creates a new digital storage
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DigitalStorageDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDigitalStorageRequest request, CancellationToken cancellationToken = default)
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
        var result = await _digitalStorageService.CreateAsync(request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id },
            ApiResponse<DigitalStorageDto>.SuccessResponse(result.Value, "Digital storage created successfully"));
    }

    /// <summary>
    /// Updates a digital storage
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<DigitalStorageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateDigitalStorageRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _digitalStorageService.UpdateAsync(id, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DigitalStorageDto>.SuccessResponse(result.Value!, "Digital storage updated successfully"));
    }

    /// <summary>
    /// Deletes a digital storage
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _digitalStorageService.DeleteAsync(id, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }

    /// <summary>
    /// Updates the usage statistics for a digital storage
    /// </summary>
    [HttpPatch("{id}/usage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUsage(Guid id, [FromBody] long bytesUsed, CancellationToken cancellationToken = default)
    {
        var result = await _digitalStorageService.UpdateUsageAsync(id, bytesUsed, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }
}
