using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for statistics operations
/// </summary>
[ApiController]
[Route("api/v1/statistics")]
[Produces("application/json")]
public class StatisticsController : ControllerBase
{
    private readonly IStatisticsService _statisticsService;
    private readonly ILogger<StatisticsController> _logger;

    public StatisticsController(IStatisticsService statisticsService, ILogger<StatisticsController> logger)
    {
        _statisticsService = statisticsService;
        _logger = logger;
    }

    /// <summary>
    /// Gets document statistics
    /// </summary>
    [HttpGet("documents")]
    [ProducesResponseType(typeof(ApiResponse<DocumentStatisticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDocumentStatistics([FromQuery] string? enterpriseCode = null, CancellationToken cancellationToken = default)
    {
        var result = await _statisticsService.GetDocumentStatisticsAsync(enterpriseCode, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentStatisticsDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets storage statistics
    /// </summary>
    [HttpGet("storage")]
    [ProducesResponseType(typeof(ApiResponse<StorageStatisticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStorageStatistics([FromQuery] string? enterpriseCode = null, CancellationToken cancellationToken = default)
    {
        var result = await _statisticsService.GetStorageStatisticsAsync(enterpriseCode, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<StorageStatisticsDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets loan statistics
    /// </summary>
    [HttpGet("loans")]
    [ProducesResponseType(typeof(ApiResponse<LoanStatisticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLoanStatistics([FromQuery] string? enterpriseCode = null, CancellationToken cancellationToken = default)
    {
        var result = await _statisticsService.GetLoanStatisticsAsync(enterpriseCode, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<LoanStatisticsDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets all statistics
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllStatistics([FromQuery] string? enterpriseCode = null, CancellationToken cancellationToken = default)
    {
        var documentStatsTask = _statisticsService.GetDocumentStatisticsAsync(enterpriseCode, cancellationToken);
        var storageStatsTask = _statisticsService.GetStorageStatisticsAsync(enterpriseCode, cancellationToken);
        var loanStatsTask = _statisticsService.GetLoanStatisticsAsync(enterpriseCode, cancellationToken);

        await Task.WhenAll(documentStatsTask, storageStatsTask, loanStatsTask);

        var result = new
        {
            Documents = documentStatsTask.Result.IsSuccess ? documentStatsTask.Result.Value : null,
            Storage = storageStatsTask.Result.IsSuccess ? storageStatsTask.Result.Value : null,
            Loans = loanStatsTask.Result.IsSuccess ? loanStatsTask.Result.Value : null
        };

        return Ok(ApiResponse<object>.SuccessResponse(result));
    }
}
