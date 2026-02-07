using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.Enterprise.Api.Application.DTOs;
using AXDD.Services.Enterprise.Api.Application.Services.Interfaces;
using AXDD.Services.Enterprise.Api.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Enterprise.Api.Controllers;

/// <summary>
/// Controller for enterprise management
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class EnterprisesController : ControllerBase
{
    private readonly IEnterpriseService _enterpriseService;
    private readonly IContactPersonService _contactService;
    private readonly IEnterpriseLicenseService _licenseService;
    private readonly IEnterpriseHistoryService _historyService;
    private readonly ILogger<EnterprisesController> _logger;

    public EnterprisesController(
        IEnterpriseService enterpriseService,
        IContactPersonService contactService,
        IEnterpriseLicenseService licenseService,
        IEnterpriseHistoryService historyService,
        ILogger<EnterprisesController> logger)
    {
        _enterpriseService = enterpriseService;
        _contactService = contactService;
        _licenseService = licenseService;
        _historyService = historyService;
        _logger = logger;
    }

    /// <summary>
    /// Gets a paginated list of enterprises with optional filtering and sorting
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 20, max: 100)</param>
    /// <param name="searchTerm">Search term for name or tax code</param>
    /// <param name="status">Filter by status</param>
    /// <param name="zoneId">Filter by industrial zone ID</param>
    /// <param name="industryCode">Filter by industry code</param>
    /// <param name="sortBy">Sort field (default: name)</param>
    /// <param name="descending">Sort descending (default: false)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EnterpriseListDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EnterpriseListDto>>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<PagedResult<EnterpriseListDto>>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] EnterpriseStatus? status = null,
        [FromQuery] Guid? zoneId = null,
        [FromQuery] string? industryCode = null,
        [FromQuery] string sortBy = "name",
        [FromQuery] bool descending = false,
        CancellationToken cancellationToken = default)
    {
        pageSize = Math.Min(pageSize, 100);

        var result = await _enterpriseService.GetAllAsync(
            pageNumber, pageSize, searchTerm, status, zoneId, industryCode, sortBy, descending, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<PagedResult<EnterpriseListDto>>.Failure(result.Error ?? "Failed to retrieve enterprises"));
        }

        return Ok(ApiResponse<PagedResult<EnterpriseListDto>>.Success(result.Value!));
    }

    /// <summary>
    /// Gets an enterprise by ID
    /// </summary>
    /// <param name="id">Enterprise ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnterpriseDto>>> GetById(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _enterpriseService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<EnterpriseDto>.NotFound(result.Error ?? "Enterprise not found"));
        }

        return Ok(ApiResponse<EnterpriseDto>.Success(result.Value!));
    }

    /// <summary>
    /// Gets an enterprise by code
    /// </summary>
    /// <param name="code">Enterprise code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnterpriseDto>>> GetByCode(
        string code,
        CancellationToken cancellationToken = default)
    {
        var result = await _enterpriseService.GetByCodeAsync(code, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<EnterpriseDto>.NotFound(result.Error ?? "Enterprise not found"));
        }

        return Ok(ApiResponse<EnterpriseDto>.Success(result.Value!));
    }

    /// <summary>
    /// Gets an enterprise by tax code
    /// </summary>
    /// <param name="taxCode">Tax code</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("taxcode/{taxCode}")]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnterpriseDto>>> GetByTaxCode(
        string taxCode,
        CancellationToken cancellationToken = default)
    {
        var result = await _enterpriseService.GetByTaxCodeAsync(taxCode, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<EnterpriseDto>.NotFound(result.Error ?? "Enterprise not found"));
        }

        return Ok(ApiResponse<EnterpriseDto>.Success(result.Value!));
    }

    /// <summary>
    /// Creates a new enterprise
    /// </summary>
    /// <param name="request">Create enterprise request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<EnterpriseDto>>> Create(
        [FromBody] CreateEnterpriseRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "system";
        var result = await _enterpriseService.CreateAsync(request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<EnterpriseDto>.Failure(result.Error ?? "Failed to create enterprise"));
        }

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.Value!.Id },
            ApiResponse<EnterpriseDto>.Success(result.Value!, "Enterprise created successfully", 201));
    }

    /// <summary>
    /// Updates an existing enterprise
    /// </summary>
    /// <param name="id">Enterprise ID</param>
    /// <param name="request">Update enterprise request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnterpriseDto>>> Update(
        Guid id,
        [FromBody] UpdateEnterpriseRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "system";
        var result = await _enterpriseService.UpdateAsync(id, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(ApiResponse<EnterpriseDto>.NotFound(result.Error));
            }
            return BadRequest(ApiResponse<EnterpriseDto>.Failure(result.Error ?? "Failed to update enterprise"));
        }

        return Ok(ApiResponse<EnterpriseDto>.Success(result.Value!, "Enterprise updated successfully"));
    }

    /// <summary>
    /// Deletes an enterprise (soft delete)
    /// </summary>
    /// <param name="id">Enterprise ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "system";
        var result = await _enterpriseService.DeleteAsync(id, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(ApiResponse<bool>.NotFound(result.Error));
            }
            return BadRequest(ApiResponse<bool>.Failure(result.Error ?? "Failed to delete enterprise"));
        }

        return Ok(ApiResponse<bool>.Success(true, "Enterprise deleted successfully"));
    }

    /// <summary>
    /// Changes the status of an enterprise
    /// </summary>
    /// <param name="id">Enterprise ID</param>
    /// <param name="request">Change status request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpPost("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EnterpriseDto>>> ChangeStatus(
        Guid id,
        [FromBody] ChangeStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "system";
        var result = await _enterpriseService.ChangeStatusAsync(id, request.NewStatus, request.Reason, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error?.Contains("not found") == true)
            {
                return NotFound(ApiResponse<EnterpriseDto>.NotFound(result.Error));
            }
            return BadRequest(ApiResponse<EnterpriseDto>.Failure(result.Error ?? "Failed to change status"));
        }

        return Ok(ApiResponse<EnterpriseDto>.Success(result.Value!, "Status changed successfully"));
    }

    /// <summary>
    /// Gets enterprise statistics
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<EnterpriseStatisticsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<EnterpriseStatisticsDto>>> GetStatistics(
        CancellationToken cancellationToken = default)
    {
        var result = await _enterpriseService.GetStatisticsAsync(cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<EnterpriseStatisticsDto>.Failure(result.Error ?? "Failed to get statistics"));
        }

        return Ok(ApiResponse<EnterpriseStatisticsDto>.Success(result.Value!));
    }

    /// <summary>
    /// Gets contacts for an enterprise
    /// </summary>
    /// <param name="id">Enterprise ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id:guid}/contacts")]
    [ProducesResponseType(typeof(ApiResponse<List<ContactPersonDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<ContactPersonDto>>>> GetContacts(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _contactService.GetByEnterpriseAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<List<ContactPersonDto>>.Failure(result.Error ?? "Failed to get contacts"));
        }

        return Ok(ApiResponse<List<ContactPersonDto>>.Success(result.Value!.ToList()));
    }

    /// <summary>
    /// Gets licenses for an enterprise
    /// </summary>
    /// <param name="id">Enterprise ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id:guid}/licenses")]
    [ProducesResponseType(typeof(ApiResponse<List<EnterpriseLicenseDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<List<EnterpriseLicenseDto>>>> GetLicenses(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var result = await _licenseService.GetByEnterpriseAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<List<EnterpriseLicenseDto>>.Failure(result.Error ?? "Failed to get licenses"));
        }

        return Ok(ApiResponse<List<EnterpriseLicenseDto>>.Success(result.Value!.ToList()));
    }

    /// <summary>
    /// Gets history for an enterprise
    /// </summary>
    /// <param name="id">Enterprise ID</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    [HttpGet("{id:guid}/history")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EnterpriseHistoryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<PagedResult<EnterpriseHistoryDto>>>> GetHistory(
        Guid id,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _historyService.GetHistoryAsync(id, pageNumber, pageSize, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<PagedResult<EnterpriseHistoryDto>>.Failure(result.Error ?? "Failed to get history"));
        }

        return Ok(ApiResponse<PagedResult<EnterpriseHistoryDto>>.Success(result.Value!));
    }
}
