using AXDD.WebApp.Admin.Models.ApiModels;
using System.Web;

namespace AXDD.WebApp.Admin.Services;

/// <summary>
/// Interface for Document Profile API service
/// </summary>
public interface IDocumentProfileApiService
{
    // Document Profile operations
    Task<ApiResponse<PagedResult<DocumentProfileDto>>> GetProfilesAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? enterpriseCode = null,
        Guid? parentProfileId = null,
        string? profileType = null,
        string? status = null,
        string? searchTerm = null,
        bool? isTemplate = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<DocumentProfileDto>> GetProfileByIdAsync(Guid profileId, CancellationToken cancellationToken = default);

    Task<ApiResponse<DocumentProfileDto>> CreateProfileAsync(CreateDocumentProfileRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<DocumentProfileDto>> UpdateProfileAsync(Guid profileId, UpdateDocumentProfileRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteProfileAsync(Guid profileId, CancellationToken cancellationToken = default);

    Task<ApiResponse<List<ProfileHierarchyDto>>> GetProfileHierarchyAsync(
        string? enterpriseCode = null,
        bool? isTemplate = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<DocumentProfileDto>> CreateProfileFromTemplateAsync(Guid templateId, CreateDocumentProfileRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> OpenProfileAsync(Guid profileId, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> CloseProfileAsync(Guid profileId, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> ArchiveProfileAsync(Guid profileId, CancellationToken cancellationToken = default);

    // Profile Metadata Field operations
    Task<ApiResponse<List<ProfileMetadataFieldDto>>> GetMetadataFieldsAsync(Guid profileId, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProfileMetadataFieldDto>> GetMetadataFieldByIdAsync(Guid fieldId, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProfileMetadataFieldDto>> CreateMetadataFieldAsync(CreateProfileMetadataFieldRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<ProfileMetadataFieldDto>> UpdateMetadataFieldAsync(Guid fieldId, UpdateProfileMetadataFieldRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteMetadataFieldAsync(Guid fieldId, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> ReorderMetadataFieldsAsync(Guid profileId, ReorderRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> CopyMetadataFieldsAsync(CopyFieldsRequest request, CancellationToken cancellationToken = default);

    // Document Profile Document operations
    Task<ApiResponse<PagedResult<DocumentProfileDocumentDto>>> GetProfileDocumentsAsync(
        Guid? profileId = null,
        int pageNumber = 1,
        int pageSize = 100,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<DocumentProfileDocumentDto>> GetProfileDocumentByIdAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<ApiResponse<DocumentProfileDocumentDto>> AddDocumentToProfileAsync(AddDocumentToProfileRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<DocumentProfileDocumentDto>> UpdateProfileDocumentAsync(Guid documentId, UpdateDocumentProfileDocumentRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> DeleteProfileDocumentAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<ApiResponse<List<DocumentMetadataValueDto>>> GetDocumentMetadataAsync(Guid documentId, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> SetDocumentMetadataAsync(Guid documentId, List<SetMetadataValueRequest> values, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> MoveDocumentAsync(Guid documentId, MoveDocumentRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<DocumentProfileDocumentDto>> CopyDocumentAsync(Guid documentId, CopyDocumentRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<bool>> ReorderProfileDocumentsAsync(Guid profileId, ReorderRequest request, CancellationToken cancellationToken = default);

    Task<ApiResponse<PagedResult<DocumentProfileDocumentDto>>> GetExpiringDocumentsAsync(
        int daysThreshold = 30,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Document Profile API service implementation
/// </summary>
public class DocumentProfileApiService : IDocumentProfileApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DocumentProfileApiService> _logger;

    public DocumentProfileApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DocumentProfileApiService> logger)
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

    // Document Profile operations
    public async Task<ApiResponse<PagedResult<DocumentProfileDto>>> GetProfilesAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? enterpriseCode = null,
        Guid? parentProfileId = null,
        string? profileType = null,
        string? status = null,
        string? searchTerm = null,
        bool? isTemplate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["pageNumber"] = pageNumber.ToString();
            queryParams["pageSize"] = pageSize.ToString();
            if (!string.IsNullOrEmpty(enterpriseCode)) queryParams["enterpriseCode"] = enterpriseCode;
            if (parentProfileId.HasValue) queryParams["parentProfileId"] = parentProfileId.Value.ToString();
            if (!string.IsNullOrEmpty(profileType)) queryParams["profileType"] = profileType;
            if (!string.IsNullOrEmpty(status)) queryParams["status"] = status;
            if (!string.IsNullOrEmpty(searchTerm)) queryParams["searchTerm"] = searchTerm;
            if (isTemplate.HasValue) queryParams["isTemplate"] = isTemplate.Value.ToString();

            var url = $"/api/v1/document-profiles?{queryParams}";
            var response = await _httpClient.GetAsync<ApiResponse<PagedResult<DocumentProfileDto>>>(url, cancellationToken);

            return response ?? new ApiResponse<PagedResult<DocumentProfileDto>>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document profiles");
            return new ApiResponse<PagedResult<DocumentProfileDto>>
            {
                Success = false,
                Message = "Unable to retrieve document profiles",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<DocumentProfileDto>> GetProfileByIdAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync<ApiResponse<DocumentProfileDto>>($"/api/v1/document-profiles/{profileId}", cancellationToken);

            return response ?? new ApiResponse<DocumentProfileDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document profile {ProfileId}", profileId);
            return new ApiResponse<DocumentProfileDto>
            {
                Success = false,
                Message = "Unable to retrieve document profile",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<DocumentProfileDto>> CreateProfileAsync(CreateDocumentProfileRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/v1/document-profiles", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<DocumentProfileDto>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<DocumentProfileDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating document profile");
            return new ApiResponse<DocumentProfileDto>
            {
                Success = false,
                Message = "Unable to create document profile",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<DocumentProfileDto>> UpdateProfileAsync(Guid profileId, UpdateDocumentProfileRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"/api/v1/document-profiles/{profileId}", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<DocumentProfileDto>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<DocumentProfileDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document profile {ProfileId}", profileId);
            return new ApiResponse<DocumentProfileDto>
            {
                Success = false,
                Message = "Unable to update document profile",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> DeleteProfileAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var success = await _httpClient.DeleteAsyncWithResult($"/api/v1/document-profiles/{profileId}", cancellationToken);

            return new ApiResponse<bool>
            {
                Success = success,
                Data = success,
                Message = success ? "Document profile deleted successfully" : "Failed to delete document profile"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document profile {ProfileId}", profileId);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to delete document profile",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<ProfileHierarchyDto>>> GetProfileHierarchyAsync(
        string? enterpriseCode = null,
        bool? isTemplate = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(enterpriseCode)) queryParams["enterpriseCode"] = enterpriseCode;
            if (isTemplate.HasValue) queryParams["isTemplate"] = isTemplate.Value.ToString();

            var url = $"/api/v1/document-profiles/hierarchy?{queryParams}";
            var response = await _httpClient.GetAsync<ApiResponse<List<ProfileHierarchyDto>>>(url, cancellationToken);

            return response ?? new ApiResponse<List<ProfileHierarchyDto>>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile hierarchy");
            return new ApiResponse<List<ProfileHierarchyDto>>
            {
                Success = false,
                Message = "Unable to retrieve profile hierarchy",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<DocumentProfileDto>> CreateProfileFromTemplateAsync(Guid templateId, CreateDocumentProfileRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/api/v1/document-profiles/from-template/{templateId}", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<DocumentProfileDto>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<DocumentProfileDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating profile from template {TemplateId}", templateId);
            return new ApiResponse<DocumentProfileDto>
            {
                Success = false,
                Message = "Unable to create profile from template",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> OpenProfileAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsync($"/api/v1/document-profiles/{profileId}/open", null, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening profile {ProfileId}", profileId);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to open profile",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> CloseProfileAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsync($"/api/v1/document-profiles/{profileId}/close", null, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing profile {ProfileId}", profileId);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to close profile",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> ArchiveProfileAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsync($"/api/v1/document-profiles/{profileId}/archive", null, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error archiving profile {ProfileId}", profileId);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to archive profile",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    // Profile Metadata Field operations
    public async Task<ApiResponse<List<ProfileMetadataFieldDto>>> GetMetadataFieldsAsync(Guid profileId, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync<ApiResponse<List<ProfileMetadataFieldDto>>>($"/api/v1/profile-metadata-fields/by-profile/{profileId}", cancellationToken);

            return response ?? new ApiResponse<List<ProfileMetadataFieldDto>>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metadata fields for profile {ProfileId}", profileId);
            return new ApiResponse<List<ProfileMetadataFieldDto>>
            {
                Success = false,
                Message = "Unable to retrieve metadata fields",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<ProfileMetadataFieldDto>> GetMetadataFieldByIdAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync<ApiResponse<ProfileMetadataFieldDto>>($"/api/v1/profile-metadata-fields/{fieldId}", cancellationToken);

            return response ?? new ApiResponse<ProfileMetadataFieldDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting metadata field {FieldId}", fieldId);
            return new ApiResponse<ProfileMetadataFieldDto>
            {
                Success = false,
                Message = "Unable to retrieve metadata field",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<ProfileMetadataFieldDto>> CreateMetadataFieldAsync(CreateProfileMetadataFieldRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/v1/profile-metadata-fields", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<ProfileMetadataFieldDto>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<ProfileMetadataFieldDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating metadata field");
            return new ApiResponse<ProfileMetadataFieldDto>
            {
                Success = false,
                Message = "Unable to create metadata field",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<ProfileMetadataFieldDto>> UpdateMetadataFieldAsync(Guid fieldId, UpdateProfileMetadataFieldRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"/api/v1/profile-metadata-fields/{fieldId}", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<ProfileMetadataFieldDto>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<ProfileMetadataFieldDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating metadata field {FieldId}", fieldId);
            return new ApiResponse<ProfileMetadataFieldDto>
            {
                Success = false,
                Message = "Unable to update metadata field",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> DeleteMetadataFieldAsync(Guid fieldId, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var success = await _httpClient.DeleteAsyncWithResult($"/api/v1/profile-metadata-fields/{fieldId}", cancellationToken);

            return new ApiResponse<bool>
            {
                Success = success,
                Data = success,
                Message = success ? "Metadata field deleted successfully" : "Failed to delete metadata field"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting metadata field {FieldId}", fieldId);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to delete metadata field",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> ReorderMetadataFieldsAsync(Guid profileId, ReorderRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/api/v1/profile-metadata-fields/by-profile/{profileId}/reorder", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering metadata fields for profile {ProfileId}", profileId);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to reorder metadata fields",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> CopyMetadataFieldsAsync(CopyFieldsRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/v1/profile-metadata-fields/copy", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying metadata fields");
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to copy metadata fields",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    // Document Profile Document operations
    public async Task<ApiResponse<PagedResult<DocumentProfileDocumentDto>>> GetProfileDocumentsAsync(
        Guid? profileId = null,
        int pageNumber = 1,
        int pageSize = 100,
        CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            if (profileId.HasValue) queryParams["profileId"] = profileId.Value.ToString();
            queryParams["pageNumber"] = pageNumber.ToString();
            queryParams["pageSize"] = pageSize.ToString();

            var url = $"/api/v1/document-profile-documents?{queryParams}";
            var response = await _httpClient.GetAsync<ApiResponse<PagedResult<DocumentProfileDocumentDto>>>(url, cancellationToken);

            return response ?? new ApiResponse<PagedResult<DocumentProfileDocumentDto>>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile documents");
            return new ApiResponse<PagedResult<DocumentProfileDocumentDto>>
            {
                Success = false,
                Message = "Unable to retrieve profile documents",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<DocumentProfileDocumentDto>> GetProfileDocumentByIdAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync<ApiResponse<DocumentProfileDocumentDto>>($"/api/v1/document-profile-documents/{documentId}", cancellationToken);

            return response ?? new ApiResponse<DocumentProfileDocumentDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile document {DocumentId}", documentId);
            return new ApiResponse<DocumentProfileDocumentDto>
            {
                Success = false,
                Message = "Unable to retrieve profile document",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<DocumentProfileDocumentDto>> AddDocumentToProfileAsync(AddDocumentToProfileRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync("/api/v1/document-profile-documents", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<DocumentProfileDocumentDto>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<DocumentProfileDocumentDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding document to profile");
            return new ApiResponse<DocumentProfileDocumentDto>
            {
                Success = false,
                Message = "Unable to add document to profile",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<DocumentProfileDocumentDto>> UpdateProfileDocumentAsync(Guid documentId, UpdateDocumentProfileDocumentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"/api/v1/document-profile-documents/{documentId}", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<DocumentProfileDocumentDto>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<DocumentProfileDocumentDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile document {DocumentId}", documentId);
            return new ApiResponse<DocumentProfileDocumentDto>
            {
                Success = false,
                Message = "Unable to update profile document",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> DeleteProfileDocumentAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var success = await _httpClient.DeleteAsyncWithResult($"/api/v1/document-profile-documents/{documentId}", cancellationToken);

            return new ApiResponse<bool>
            {
                Success = success,
                Data = success,
                Message = success ? "Document removed from profile successfully" : "Failed to remove document from profile"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting profile document {DocumentId}", documentId);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to remove document from profile",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<DocumentMetadataValueDto>>> GetDocumentMetadataAsync(Guid documentId, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync<ApiResponse<List<DocumentMetadataValueDto>>>($"/api/v1/document-profile-documents/{documentId}/metadata", cancellationToken);

            return response ?? new ApiResponse<List<DocumentMetadataValueDto>>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting document metadata {DocumentId}", documentId);
            return new ApiResponse<List<DocumentMetadataValueDto>>
            {
                Success = false,
                Message = "Unable to retrieve document metadata",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> SetDocumentMetadataAsync(Guid documentId, List<SetMetadataValueRequest> values, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var json = System.Text.Json.JsonSerializer.Serialize(values);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PutAsync($"/api/v1/document-profile-documents/{documentId}/metadata", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting document metadata {DocumentId}", documentId);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to set document metadata",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> MoveDocumentAsync(Guid documentId, MoveDocumentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/api/v1/document-profile-documents/{documentId}/move", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving document {DocumentId}", documentId);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to move document",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<DocumentProfileDocumentDto>> CopyDocumentAsync(Guid documentId, CopyDocumentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/api/v1/document-profile-documents/{documentId}/copy", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<DocumentProfileDocumentDto>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<DocumentProfileDocumentDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error copying document {DocumentId}", documentId);
            return new ApiResponse<DocumentProfileDocumentDto>
            {
                Success = false,
                Message = "Unable to copy document",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> ReorderProfileDocumentsAsync(Guid profileId, ReorderRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var json = System.Text.Json.JsonSerializer.Serialize(request);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            
            var response = await _httpClient.PostAsync($"/api/v1/document-profile-documents/by-profile/{profileId}/reorder", content, cancellationToken);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = System.Text.Json.JsonSerializer.Deserialize<ApiResponse<bool>>(responseContent,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return result ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering profile documents for profile {ProfileId}", profileId);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to reorder profile documents",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<PagedResult<DocumentProfileDocumentDto>>> GetExpiringDocumentsAsync(
        int daysThreshold = 30,
        int pageNumber = 1,
        int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["daysThreshold"] = daysThreshold.ToString();
            queryParams["pageNumber"] = pageNumber.ToString();
            queryParams["pageSize"] = pageSize.ToString();

            var url = $"/api/v1/document-profile-documents/expiring?{queryParams}";
            var response = await _httpClient.GetAsync<ApiResponse<PagedResult<DocumentProfileDocumentDto>>>(url, cancellationToken);

            return response ?? new ApiResponse<PagedResult<DocumentProfileDocumentDto>>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting expiring documents");
            return new ApiResponse<PagedResult<DocumentProfileDocumentDto>>
            {
                Success = false,
                Message = "Unable to retrieve expiring documents",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}
