using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for storage quota operations
/// </summary>
[ApiController]
[Route("api/v1/quota")]
[Produces("application/json")]
public class StorageQuotaController : ControllerBase
{
    private readonly IStorageQuotaService _quotaService;
    private readonly ILogger<StorageQuotaController> _logger;

    /// <summary>
    /// Initializes a new instance of StorageQuotaController
    /// </summary>
    public StorageQuotaController(
        IStorageQuotaService quotaService,
        ILogger<StorageQuotaController> logger)
    {
        _quotaService = quotaService;
        _logger = logger;
    }

    /// <summary>
    /// Gets storage quota for an enterprise
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The storage quota</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<StorageQuotaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetQuota(
        [FromQuery] string enterpriseCode,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(enterpriseCode))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Enterprise code is required"));
        }

        var result = await _quotaService.GetQuotaAsync(enterpriseCode, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<StorageQuotaDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Initializes storage quota for an enterprise
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="quotaGB">Optional quota in GB (defaults to configured value)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The initialized storage quota</returns>
    [HttpPost("initialize")]
    [ProducesResponseType(typeof(ApiResponse<StorageQuotaDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> InitializeQuota(
        [FromQuery] string enterpriseCode,
        [FromQuery] long? quotaGB = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(enterpriseCode))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Enterprise code is required"));
        }

        long? quotaBytes = quotaGB.HasValue ? quotaGB.Value * 1024 * 1024 * 1024 : null;

        var result = await _quotaService.InitializeQuotaAsync(enterpriseCode, quotaBytes, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return CreatedAtAction(
            nameof(GetQuota),
            new { enterpriseCode },
            ApiResponse<StorageQuotaDto>.SuccessResponse(result.Value!, "Quota initialized successfully"));
    }
}
