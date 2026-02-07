using AXDD.WebApp.Admin.Models.ApiModels;
using System.Web;

namespace AXDD.WebApp.Admin.Services;

/// <summary>
/// Interface for document API service
/// </summary>
public interface IDocumentApiService
{
    /// <summary>
    /// Get documents with pagination and filtering
    /// </summary>
    Task<ApiResponse<PagedResult<DocumentDto>>> GetDocumentsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        Guid? enterpriseId = null,
        string? documentType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get document by ID
    /// </summary>
    Task<ApiResponse<DocumentDto>> GetDocumentByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Upload document
    /// </summary>
    Task<ApiResponse<DocumentDto>> UploadDocumentAsync(
        Guid enterpriseId,
        IFormFile file,
        string documentType,
        string? description = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Download document
    /// </summary>
    Task<byte[]?> DownloadDocumentAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete document
    /// </summary>
    Task<ApiResponse<bool>> DeleteDocumentAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Document API service implementation
/// </summary>
public class DocumentApiService : IDocumentApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DocumentApiService> _logger;

    public DocumentApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DocumentApiService> logger)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    private void AddAuthorizationHeader()
    {
        var token = _httpContextAccessor.HttpContext?.Request.Cookies["AuthToken"];
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.AddBearerToken(token);
        }
    }

    public async Task<ApiResponse<PagedResult<DocumentDto>>> GetDocumentsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        Guid? enterpriseId = null,
        string? documentType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["pageNumber"] = pageNumber.ToString();
            queryParams["pageSize"] = pageSize.ToString();
            if (enterpriseId.HasValue) queryParams["enterpriseId"] = enterpriseId.Value.ToString();
            if (!string.IsNullOrEmpty(documentType)) queryParams["documentType"] = documentType;

            var url = $"/api/v1/documents?{queryParams}";
            var response = await _httpClient.GetAsync<ApiResponse<PagedResult<DocumentDto>>>(url, cancellationToken);

            return response ?? new ApiResponse<PagedResult<DocumentDto>>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting documents");
            return new ApiResponse<PagedResult<DocumentDto>>
            {
                Success = false,
                Message = "Unable to retrieve documents",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<DocumentDto>> GetDocumentByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync<ApiResponse<DocumentDto>>($"/api/v1/documents/{id}", cancellationToken);

            return response ?? new ApiResponse<DocumentDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document {DocumentId}", id);
            return new ApiResponse<DocumentDto>
            {
                Success = false,
                Message = "Unable to retrieve document",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<DocumentDto>> UploadDocumentAsync(
        Guid enterpriseId,
        IFormFile file,
        string documentType,
        string? description = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();

            using var content = new MultipartFormDataContent();
            using var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.FileName);
            content.Add(new StringContent(enterpriseId.ToString()), "enterpriseId");
            content.Add(new StringContent(documentType), "documentType");
            if (!string.IsNullOrEmpty(description))
            {
                content.Add(new StringContent(description), "description");
            }

            var response = await _httpClient.PostAsync("/api/v1/documents/upload", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<DocumentDto>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<DocumentDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading document");
            return new ApiResponse<DocumentDto>
            {
                Success = false,
                Message = "Unable to upload document",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<byte[]?> DownloadDocumentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync($"/api/v1/documents/{id}/download", cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading document {DocumentId}", id);
            return null;
        }
    }

    public async Task<ApiResponse<bool>> DeleteDocumentAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var success = await _httpClient.DeleteAsyncWithResult($"/api/v1/documents/{id}", cancellationToken);

            return new ApiResponse<bool>
            {
                Success = success,
                Data = success,
                Message = success ? "Document deleted successfully" : "Failed to delete document"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {DocumentId}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to delete document",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}
