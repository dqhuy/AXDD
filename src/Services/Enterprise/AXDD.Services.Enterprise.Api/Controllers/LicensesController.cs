using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.Enterprise.Api.Application.DTOs;
using AXDD.Services.Enterprise.Api.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Enterprise.Api.Controllers;

/// <summary>
/// Controller for enterprise license management
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class LicensesController : ControllerBase
{
    private readonly IEnterpriseLicenseService _licenseService;
    private readonly ILogger<LicensesController> _logger;

    public LicensesController(
        IEnterpriseLicenseService licenseService,
        ILogger<LicensesController> logger)
    {
        _licenseService = licenseService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a license by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseLicenseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseLicenseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnterpriseLicenseDto>>> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _licenseService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<EnterpriseLicenseDto>.NotFound(result.Error ?? "License not found"));
        }

        return Ok(ApiResponse<EnterpriseLicenseDto>.Success(result.Value!));
    }

    /// <summary>
    /// Creates a new license
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseLicenseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseLicenseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<EnterpriseLicenseDto>>> Create(
        [FromBody] CreateLicenseRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "system";
        var result = await _licenseService.CreateAsync(request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<EnterpriseLicenseDto>.Failure(result.Error ?? "Failed to create license"));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            ApiResponse<EnterpriseLicenseDto>.Success(result.Value!, "License created successfully", 201));
    }

    /// <summary>
    /// Updates an existing license
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseLicenseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseLicenseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseLicenseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnterpriseLicenseDto>>> Update(
        Guid id,
        [FromBody] UpdateLicenseRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "system";
        var result = await _licenseService.UpdateAsync(id, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(ApiResponse<EnterpriseLicenseDto>.NotFound(result.Error));
            }
            return BadRequest(ApiResponse<EnterpriseLicenseDto>.Failure(result.Error ?? "Failed to update license"));
        }

        return Ok(ApiResponse<EnterpriseLicenseDto>.Success(result.Value!, "License updated successfully"));
    }

    /// <summary>
    /// Deletes a license
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "system";
        var result = await _licenseService.DeleteAsync(id, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(ApiResponse<bool>.NotFound(result.Error));
            }
            return BadRequest(ApiResponse<bool>.Failure(result.Error ?? "Failed to delete license"));
        }

        return Ok(ApiResponse<bool>.Success(true, "License deleted successfully"));
    }

    /// <summary>
    /// Gets licenses expiring within specified days
    /// </summary>
    [HttpGet("expiring")]
    [ProducesResponseType(typeof(ApiResponse<List<EnterpriseLicenseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<EnterpriseLicenseDto>>>> GetExpiring(
        [FromQuery] int days = 30,
        CancellationToken cancellationToken = default)
    {
        var result = await _licenseService.GetExpiringLicensesAsync(days, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<List<EnterpriseLicenseDto>>.Failure(result.Error ?? "Failed to get expiring licenses"));
        }

        return Ok(ApiResponse<List<EnterpriseLicenseDto>>.Success(result.Value!.ToList()));
    }
}
