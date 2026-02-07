using AXDD.WebApp.Admin.Models.ApiModels;
using System.Web;

namespace AXDD.WebApp.Admin.Services;

/// <summary>
/// Interface for enterprise API service
/// </summary>
public interface IEnterpriseApiService
{
    /// <summary>
    /// Get all enterprises with pagination and filtering
    /// </summary>
    Task<ApiResponse<PagedResult<EnterpriseDto>>> GetEnterprisesAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? status = null,
        string? type = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get enterprise by ID
    /// </summary>
    Task<ApiResponse<EnterpriseDto>> GetEnterpriseByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create new enterprise
    /// </summary>
    Task<ApiResponse<EnterpriseDto>> CreateEnterpriseAsync(EnterpriseDto enterprise, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update enterprise
    /// </summary>
    Task<ApiResponse<EnterpriseDto>> UpdateEnterpriseAsync(Guid id, EnterpriseDto enterprise, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete enterprise
    /// </summary>
    Task<ApiResponse<bool>> DeleteEnterpriseAsync(Guid id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Enterprise API service implementation
/// </summary>
public class EnterpriseApiService : IEnterpriseApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<EnterpriseApiService> _logger;

    public EnterpriseApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<EnterpriseApiService> logger)
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

    public async Task<ApiResponse<PagedResult<EnterpriseDto>>> GetEnterprisesAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        string? status = null,
        string? type = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["pageNumber"] = pageNumber.ToString();
            queryParams["pageSize"] = pageSize.ToString();
            if (!string.IsNullOrEmpty(searchTerm)) queryParams["searchTerm"] = searchTerm;
            if (!string.IsNullOrEmpty(status)) queryParams["status"] = status;
            if (!string.IsNullOrEmpty(type)) queryParams["type"] = type;

            var url = $"/api/v1/enterprises?{queryParams}";
            var response = await _httpClient.GetAsync<ApiResponse<PagedResult<EnterpriseDto>>>(url, cancellationToken);

            return response ?? new ApiResponse<PagedResult<EnterpriseDto>>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting enterprises");
            return new ApiResponse<PagedResult<EnterpriseDto>>
            {
                Success = false,
                Message = "Unable to retrieve enterprises",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<EnterpriseDto>> GetEnterpriseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync<ApiResponse<EnterpriseDto>>($"/api/v1/enterprises/{id}", cancellationToken);

            return response ?? new ApiResponse<EnterpriseDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting enterprise {EnterpriseId}", id);
            return new ApiResponse<EnterpriseDto>
            {
                Success = false,
                Message = "Unable to retrieve enterprise",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<EnterpriseDto>> CreateEnterpriseAsync(EnterpriseDto enterprise, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync<EnterpriseDto, ApiResponse<EnterpriseDto>>(
                "/api/v1/enterprises",
                enterprise,
                cancellationToken);

            return response ?? new ApiResponse<EnterpriseDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating enterprise");
            return new ApiResponse<EnterpriseDto>
            {
                Success = false,
                Message = "Unable to create enterprise",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<EnterpriseDto>> UpdateEnterpriseAsync(Guid id, EnterpriseDto enterprise, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync<EnterpriseDto, ApiResponse<EnterpriseDto>>(
                $"/api/v1/enterprises/{id}",
                enterprise,
                cancellationToken);

            return response ?? new ApiResponse<EnterpriseDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating enterprise {EnterpriseId}", id);
            return new ApiResponse<EnterpriseDto>
            {
                Success = false,
                Message = "Unable to update enterprise",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> DeleteEnterpriseAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var success = await _httpClient.DeleteAsyncWithResult($"/api/v1/enterprises/{id}", cancellationToken);

            return new ApiResponse<bool>
            {
                Success = success,
                Data = success,
                Message = success ? "Enterprise deleted successfully" : "Failed to delete enterprise"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting enterprise {EnterpriseId}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to delete enterprise",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}
