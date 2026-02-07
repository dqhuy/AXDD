using AXDD.Services.Report.Api.Application.DTOs;
using AXDD.Services.Report.Api.Application.Services.Interfaces;
using AXDD.Services.Report.Api.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Report.Api.Controllers;

/// <summary>
/// Controller for managing report templates
/// </summary>
[ApiController]
[Route("api/v1/report-templates")]
public class ReportTemplatesController : ControllerBase
{
    private readonly IReportTemplateService _templateService;

    public ReportTemplatesController(IReportTemplateService templateService)
    {
        _templateService = templateService;
    }

    /// <summary>
    /// Get all active report templates
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of active templates</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<ReportTemplateDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTemplatesAsync(CancellationToken ct)
    {
        var result = await _templateService.GetTemplatesAsync(ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get a template by ID
    /// </summary>
    /// <param name="id">Template ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Template details</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ReportTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTemplateByIdAsync(Guid id, CancellationToken ct)
    {
        var result = await _templateService.GetTemplateByIdAsync(id, ct);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Get a template by report type
    /// </summary>
    /// <param name="type">Report type</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Template details</returns>
    [HttpGet("by-type/{type}")]
    [ProducesResponseType(typeof(ReportTemplateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTemplateByTypeAsync(ReportType type, CancellationToken ct)
    {
        var result = await _templateService.GetTemplateByTypeAsync(type, ct);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Create a new report template (admin only)
    /// </summary>
    /// <param name="request">Template creation request</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Created template</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ReportTemplateDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTemplateAsync(
        [FromBody] CreateTemplateRequest request,
        CancellationToken ct)
    {
        // TODO: Get actual user ID from authentication and verify admin role
        var userId = "admin";

        var result = await _templateService.CreateTemplateAsync(request, userId, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(
            nameof(GetTemplateByIdAsync),
            new { id = result.Value!.Id },
            result.Value);
    }
}
