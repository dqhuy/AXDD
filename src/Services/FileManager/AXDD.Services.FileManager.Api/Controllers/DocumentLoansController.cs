using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Enums;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for document loan operations
/// </summary>
[ApiController]
[Route("api/v1/loans")]
[Produces("application/json")]
public class DocumentLoansController : ControllerBase
{
    private readonly IDocumentLoanService _loanService;
    private readonly ILogger<DocumentLoansController> _logger;

    public DocumentLoansController(IDocumentLoanService loanService, ILogger<DocumentLoansController> logger)
    {
        _loanService = loanService;
        _logger = logger;
    }

    /// <summary>
    /// Lists document loans with filters
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DocumentLoanDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        [FromQuery] string? enterpriseCode = null,
        [FromQuery] LoanStatus? status = null,
        [FromQuery] LoanType? loanType = null,
        [FromQuery] string? borrowerUserId = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _loanService.ListAsync(enterpriseCode, status, loanType, borrowerUserId, pageNumber, pageSize, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<DocumentLoanDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets a document loan by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentLoanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _loanService.GetByIdAsync(id, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentLoanDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets a document loan by loan code
    /// </summary>
    [HttpGet("code/{loanCode}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentLoanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string loanCode, CancellationToken cancellationToken = default)
    {
        var result = await _loanService.GetByCodeAsync(loanCode, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentLoanDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Creates a new document loan request
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DocumentLoanDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateDocumentLoanRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.BorrowerName))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Borrower name is required"));
        }

        if (string.IsNullOrWhiteSpace(request.EnterpriseCode))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Enterprise code is required"));
        }

        if (request.DocumentIds == null || request.DocumentIds.Count == 0)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("At least one document is required"));
        }

        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _loanService.CreateAsync(request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id },
            ApiResponse<DocumentLoanDto>.SuccessResponse(result.Value, "Document loan created successfully"));
    }

    /// <summary>
    /// Approves a document loan
    /// </summary>
    [HttpPut("{id}/approve")]
    [ProducesResponseType(typeof(ApiResponse<DocumentLoanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _loanService.ApproveAsync(id, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentLoanDto>.SuccessResponse(result.Value!, "Document loan approved successfully"));
    }

    /// <summary>
    /// Rejects a document loan
    /// </summary>
    [HttpPut("{id}/reject")]
    [ProducesResponseType(typeof(ApiResponse<DocumentLoanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Reject(Guid id, [FromBody] ProcessLoanRequest request, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _loanService.RejectAsync(id, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentLoanDto>.SuccessResponse(result.Value!, "Document loan rejected"));
    }

    /// <summary>
    /// Marks a document loan as borrowed
    /// </summary>
    [HttpPut("{id}/borrow")]
    [ProducesResponseType(typeof(ApiResponse<DocumentLoanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> MarkAsBorrowed(Guid id, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _loanService.MarkAsBorrowedAsync(id, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentLoanDto>.SuccessResponse(result.Value!, "Documents marked as borrowed"));
    }

    /// <summary>
    /// Returns documents for a loan
    /// </summary>
    [HttpPut("{id}/return")]
    [ProducesResponseType(typeof(ApiResponse<DocumentLoanDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReturnDocuments(Guid id, [FromBody] List<Guid>? itemIds = null, CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";
        var result = await _loanService.ReturnDocumentsAsync(id, itemIds, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentLoanDto>.SuccessResponse(result.Value!, "Documents returned successfully"));
    }

    /// <summary>
    /// Gets loan statistics
    /// </summary>
    [HttpGet("statistics")]
    [ProducesResponseType(typeof(ApiResponse<LoanStatisticsDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatistics([FromQuery] string? enterpriseCode = null, CancellationToken cancellationToken = default)
    {
        var result = await _loanService.GetStatisticsAsync(enterpriseCode, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<LoanStatisticsDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Gets overdue loans
    /// </summary>
    [HttpGet("overdue")]
    [ProducesResponseType(typeof(ApiResponse<List<DocumentLoanDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverdue([FromQuery] string? enterpriseCode = null, CancellationToken cancellationToken = default)
    {
        var result = await _loanService.GetOverdueLoansAsync(enterpriseCode, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<List<DocumentLoanDto>>.SuccessResponse(result.Value!));
    }
}
