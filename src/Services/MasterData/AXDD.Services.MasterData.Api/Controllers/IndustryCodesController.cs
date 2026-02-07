using AXDD.Services.MasterData.Api.DTOs;
using AXDD.Services.MasterData.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.MasterData.Api.Controllers;

/// <summary>
/// Controller for managing industry codes (VSIC)
/// </summary>
[ApiController]
[Route("api/v1/masterdata/[controller]")]
[Produces("application/json")]
public class IndustryCodesController : ControllerBase
{
    private readonly IIndustryCodeService _service;
    private readonly ILogger<IndustryCodesController> _logger;

    public IndustryCodesController(
        IIndustryCodeService service,
        ILogger<IndustryCodesController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Gets all industry codes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<IndustryCodeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<IndustryCodeDto>>> GetAll(
        [FromQuery] int? level = null,
        [FromQuery] string? parentCode = null,
        CancellationToken cancellationToken = default)
    {
        var codes = await _service.GetAllAsync(level, parentCode, cancellationToken);
        return Ok(codes);
    }

    /// <summary>
    /// Gets an industry code by code
    /// </summary>
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(IndustryCodeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IndustryCodeDto>> GetByCode(string code, CancellationToken cancellationToken)
    {
        var industryCode = await _service.GetByCodeAsync(code, cancellationToken);
        if (industryCode == null)
        {
            return NotFound();
        }

        return Ok(industryCode);
    }

    /// <summary>
    /// Gets the hierarchy for an industry code
    /// </summary>
    [HttpGet("{code}/hierarchy")]
    [ProducesResponseType(typeof(IndustryCodeHierarchyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IndustryCodeHierarchyDto>> GetHierarchy(string code, CancellationToken cancellationToken)
    {
        var hierarchy = await _service.GetHierarchyAsync(code, cancellationToken);
        if (hierarchy == null)
        {
            return NotFound();
        }

        return Ok(hierarchy);
    }

    /// <summary>
    /// Searches industry codes
    /// </summary>
    [HttpGet("search")]
    [ProducesResponseType(typeof(List<IndustryCodeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<IndustryCodeDto>>> Search(
        [FromQuery] string q,
        CancellationToken cancellationToken)
    {
        var codes = await _service.SearchAsync(q, cancellationToken);
        return Ok(codes);
    }
}
