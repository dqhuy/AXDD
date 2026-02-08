using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Enums;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for document approval operations
/// </summary>
[ApiController]
[Route("api/v1/document-approvals")]
[Produces("application/json")]
public class DocumentApprovalsController : ControllerBase
{
    private readonly IDocumentApprovalService _approvalService;
    private readonly ILogger<DocumentApprovalsController> _logger;

    public DocumentApprovalsController(IDocumentApprovalService approvalService, ILogger<DocumentApprovalsController> logger)
    {
        _approvalService = approvalService;
        _logger = logger;
    }

    /// <summary>
    /// Lists document approvals with filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DocumentApprovalDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] ApprovalStatus? status = null,
        [FromQuery] string? requestedBy = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _approvalService.ListAsync(status, requestedBy, pageNumber, pageSize, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<DocumentApprovalDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets a document approval by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentApprovalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _approvalService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentApprovalDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets pending approvals
    /// </summary>
    [HttpGet("pending")]
    [ProducesResponseType(typeof(ApiResponse<List<DocumentApprovalDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPending(CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _approvalService.GetPendingApprovalsAsync(userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<List<DocumentApprovalDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Submits a document for approval
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DocumentApprovalDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Submit([FromBody] SubmitApprovalRequest request, CancellationToken cancellationToken = default)
    {
        if (request.DocumentId == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("DocumentId is required"));
        }

        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _approvalService.SubmitForApprovalAsync(request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id },
            ApiResponse<DocumentApprovalDto>.SuccessResponse(result.Value, "Document submitted for approval"));
    }

    /// <summary>
    /// Approves a document
    /// </summary>
    [HttpPut("{id}/approve")]
    [ProducesResponseType(typeof(ApiResponse<DocumentApprovalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Approve(Guid id, [FromBody] ProcessApprovalRequest? request = null, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _approvalService.ApproveAsync(id, request ?? new ProcessApprovalRequest(), userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentApprovalDto>.SuccessResponse(result.Value!, "Document approved successfully"));
    }

    /// <summary>
    /// Rejects a document
    /// </summary>
    [HttpPut("{id}/reject")]
    [ProducesResponseType(typeof(ApiResponse<DocumentApprovalDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ProcessApprovalRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _approvalService.RejectAsync(id, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentApprovalDto>.SuccessResponse(result.Value!, "Document rejected"));
    }
}
