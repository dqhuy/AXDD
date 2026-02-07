using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.FileManager.Api.DTOs;
using AXDD.Services.FileManager.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.FileManager.Api.Controllers;

/// <summary>
/// Controller for document profile document operations
/// </summary>
[ApiController]
[Route("api/v1/document-profile-documents")]
[Produces("application/json")]
public class DocumentProfileDocumentsController : ControllerBase
{
    private readonly IDocumentProfileDocumentService _documentService;
    private readonly ILogger<DocumentProfileDocumentsController> _logger;

    /// <summary>
    /// Initializes a new instance of DocumentProfileDocumentsController
    /// </summary>
    public DocumentProfileDocumentsController(
        IDocumentProfileDocumentService documentService,
        ILogger<DocumentProfileDocumentsController> logger)
    {
        _documentService = documentService;
        _logger = logger;
    }

    /// <summary>
    /// Adds a document to a profile
    /// </summary>
    /// <param name="request">The add document request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added document</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<DocumentProfileDocumentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddDocument(
        [FromBody] AddDocumentToProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Document title is required"));
        }

        if (request.ProfileId == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Profile ID is required"));
        }

        if (request.FileMetadataId == Guid.Empty)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("File metadata ID is required"));
        }

        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _documentService.AddDocumentAsync(request, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return CreatedAtAction(
            nameof(GetDocument),
            new { documentId = result.Value!.Id },
            ApiResponse<DocumentProfileDocumentDto>.SuccessResponse(result.Value!, "Document added to profile successfully"));
    }

    /// <summary>
    /// Gets a document by ID
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="includeMetadata">Whether to include metadata values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The document</returns>
    [HttpGet("{documentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentProfileDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDocument(
        Guid documentId,
        [FromQuery] bool includeMetadata = false,
        CancellationToken cancellationToken = default)
    {
        var result = await _documentService.GetDocumentAsync(documentId, includeMetadata, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<DocumentProfileDocumentDto>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Updates a document
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="request">The update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated document</returns>
    [HttpPut("{documentId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<DocumentProfileDocumentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateDocument(
        Guid documentId,
        [FromBody] UpdateDocumentProfileDocumentRequest request,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _documentService.UpdateDocumentAsync(documentId, request, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error!.Contains("not found"))
            {
                return NotFound(ApiResponse<object>.ErrorResponse(result.Error));
            }
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error));
        }

        return Ok(ApiResponse<DocumentProfileDocumentDto>.SuccessResponse(result.Value!, "Document updated successfully"));
    }

    /// <summary>
    /// Removes a document from a profile
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{documentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveDocument(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _documentService.RemoveDocumentAsync(documentId, userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }

    /// <summary>
    /// Lists documents with pagination
    /// </summary>
    /// <param name="profileId">Optional profile ID filter</param>
    /// <param name="documentType">Optional document type filter</param>
    /// <param name="status">Optional status filter</param>
    /// <param name="searchTerm">Optional search term</param>
    /// <param name="issueDateFrom">Optional issue date from filter</param>
    /// <param name="issueDateTo">Optional issue date to filter</param>
    /// <param name="expiryDateFrom">Optional expiry date from filter</param>
    /// <param name="expiryDateTo">Optional expiry date to filter</param>
    /// <param name="pageNumber">Page number (default 1)</param>
    /// <param name="pageSize">Page size (default 10)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of documents</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<DocumentProfileDocumentDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListDocuments(
        [FromQuery] Guid? profileId = null,
        [FromQuery] string? documentType = null,
        [FromQuery] string? status = null,
        [FromQuery] string? searchTerm = null,
        [FromQuery] DateTime? issueDateFrom = null,
        [FromQuery] DateTime? issueDateTo = null,
        [FromQuery] DateTime? expiryDateFrom = null,
        [FromQuery] DateTime? expiryDateTo = null,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new DocumentProfileDocumentListQuery
        {
            ProfileId = profileId,
            DocumentType = documentType,
            Status = status,
            SearchTerm = searchTerm,
            IssueDateFrom = issueDateFrom,
            IssueDateTo = issueDateTo,
            ExpiryDateFrom = expiryDateFrom,
            ExpiryDateTo = expiryDateTo,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _documentService.ListDocumentsAsync(query, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<PagedResult<DocumentProfileDocumentDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Sets metadata values for a document
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="values">The metadata values to set</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated metadata values</returns>
    [HttpPut("{documentId:guid}/metadata")]
    [ProducesResponseType(typeof(ApiResponse<List<DocumentMetadataValueDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetMetadataValues(
        Guid documentId,
        [FromBody] List<SetMetadataValueRequest> values,
        CancellationToken cancellationToken = default)
    {
        if (values == null || !values.Any())
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Metadata values are required"));
        }

        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _documentService.SetMetadataValuesAsync(documentId, values, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error!.Contains("not found"))
            {
                return NotFound(ApiResponse<object>.ErrorResponse(result.Error));
            }
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error));
        }

        return Ok(ApiResponse<List<DocumentMetadataValueDto>>.SuccessResponse(result.Value!, "Metadata values updated successfully"));
    }

    /// <summary>
    /// Gets metadata values for a document
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The metadata values</returns>
    [HttpGet("{documentId:guid}/metadata")]
    [ProducesResponseType(typeof(ApiResponse<List<DocumentMetadataValueDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMetadataValues(
        Guid documentId,
        CancellationToken cancellationToken = default)
    {
        var result = await _documentService.GetMetadataValuesAsync(documentId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<List<DocumentMetadataValueDto>>.SuccessResponse(result.Value!));
    }

    /// <summary>
    /// Moves a document to a different profile
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="targetProfileId">The target profile ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPost("{documentId:guid}/move")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MoveDocument(
        Guid documentId,
        [FromQuery] Guid targetProfileId,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _documentService.MoveDocumentAsync(documentId, targetProfileId, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error!.Contains("not found"))
            {
                return NotFound(ApiResponse<object>.ErrorResponse(result.Error));
            }
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error));
        }

        return NoContent();
    }

    /// <summary>
    /// Copies a document to another profile
    /// </summary>
    /// <param name="documentId">The document ID</param>
    /// <param name="targetProfileId">The target profile ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The copied document</returns>
    [HttpPost("{documentId:guid}/copy")]
    [ProducesResponseType(typeof(ApiResponse<DocumentProfileDocumentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CopyDocument(
        Guid documentId,
        [FromQuery] Guid targetProfileId,
        CancellationToken cancellationToken = default)
    {
        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _documentService.CopyDocumentAsync(documentId, targetProfileId, userId, cancellationToken);

        if (result.IsFailure)
        {
            if (result.Error!.Contains("not found"))
            {
                return NotFound(ApiResponse<object>.ErrorResponse(result.Error));
            }
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error));
        }

        return CreatedAtAction(
            nameof(GetDocument),
            new { documentId = result.Value!.Id },
            ApiResponse<DocumentProfileDocumentDto>.SuccessResponse(result.Value!, "Document copied successfully"));
    }

    /// <summary>
    /// Reorders documents within a profile
    /// </summary>
    /// <param name="profileId">The profile ID</param>
    /// <param name="documentOrders">Dictionary of document ID to display order</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content on success</returns>
    [HttpPost("by-profile/{profileId:guid}/reorder")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ReorderDocuments(
        Guid profileId,
        [FromBody] Dictionary<Guid, int> documentOrders,
        CancellationToken cancellationToken = default)
    {
        if (documentOrders == null || !documentOrders.Any())
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Document orders are required"));
        }

        var userId = User.Identity?.Name ?? "anonymous";

        var result = await _documentService.ReorderDocumentsAsync(profileId, documentOrders, userId, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return NoContent();
    }

    /// <summary>
    /// Gets documents expiring within a specified period
    /// </summary>
    /// <param name="enterpriseCode">The enterprise code</param>
    /// <param name="daysAhead">Number of days to look ahead (default 30)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of expiring documents</returns>
    [HttpGet("expiring")]
    [ProducesResponseType(typeof(ApiResponse<List<DocumentProfileDocumentDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetExpiringDocuments(
        [FromQuery] string enterpriseCode,
        [FromQuery] int daysAhead = 30,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(enterpriseCode))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Enterprise code is required"));
        }

        var result = await _documentService.GetExpiringDocumentsAsync(enterpriseCode, daysAhead, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(result.Error!));
        }

        return Ok(ApiResponse<List<DocumentProfileDocumentDto>>.SuccessResponse(result.Value!));
    }
}
