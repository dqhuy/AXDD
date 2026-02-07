using AXDD.WebApp.Admin.Models.ApiModels;
using System.Web;

namespace AXDD.WebApp.Admin.Services;

/// <summary>
/// Interface for report API service
/// </summary>
public interface IReportApiService
{
    /// <summary>
    /// Get reports with pagination and filtering
    /// </summary>
    Task<ApiResponse<PagedResult<ReportDto>>> GetReportsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? status = null,
        string? reportType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Get report by ID
    /// </summary>
    Task<ApiResponse<ReportDto>> GetReportByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Approve report
    /// </summary>
    Task<ApiResponse<bool>> ApproveReportAsync(Guid id, string comments, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reject report
    /// </summary>
    Task<ApiResponse<bool>> RejectReportAsync(Guid id, string comments, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get report statistics
    /// </summary>
    Task<ApiResponse<List<ReportsByStatusDto>>> GetReportStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Report API service implementation
/// </summary>
public class ReportApiService : IReportApiService
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ReportApiService> _logger;

    public ReportApiService(
        HttpClient httpClient,
        IHttpContextAccessor httpContextAccessor,
        ILogger<ReportApiService> logger)
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

    public async Task<ApiResponse<PagedResult<ReportDto>>> GetReportsAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? status = null,
        string? reportType = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["pageNumber"] = pageNumber.ToString();
            queryParams["pageSize"] = pageSize.ToString();
            if (!string.IsNullOrEmpty(status)) queryParams["status"] = status;
            if (!string.IsNullOrEmpty(reportType)) queryParams["reportType"] = reportType;

            var url = $"/api/v1/reports?{queryParams}";
            var response = await _httpClient.GetAsync<ApiResponse<PagedResult<ReportDto>>>(url, cancellationToken);

            return response ?? new ApiResponse<PagedResult<ReportDto>>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reports");
            return new ApiResponse<PagedResult<ReportDto>>
            {
                Success = false,
                Message = "Unable to retrieve reports",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<ReportDto>> GetReportByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync<ApiResponse<ReportDto>>($"/api/v1/reports/{id}", cancellationToken);

            return response ?? new ApiResponse<ReportDto>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting report {ReportId}", id);
            return new ApiResponse<ReportDto>
            {
                Success = false,
                Message = "Unable to retrieve report",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> ApproveReportAsync(Guid id, string comments, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync<object, ApiResponse<bool>>(
                $"/api/v1/reports/{id}/approve",
                new { Comments = comments },
                cancellationToken);

            return response ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving report {ReportId}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to approve report",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<bool>> RejectReportAsync(Guid id, string comments, CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.PostAsJsonAsync<object, ApiResponse<bool>>(
                $"/api/v1/reports/{id}/reject",
                new { Comments = comments },
                cancellationToken);

            return response ?? new ApiResponse<bool>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting report {ReportId}", id);
            return new ApiResponse<bool>
            {
                Success = false,
                Message = "Unable to reject report",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<List<ReportsByStatusDto>>> GetReportStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            AddAuthorizationHeader();
            var response = await _httpClient.GetAsync<ApiResponse<List<ReportsByStatusDto>>>("/api/v1/reports/statistics", cancellationToken);

            return response ?? new ApiResponse<List<ReportsByStatusDto>>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting report statistics");
            return new ApiResponse<List<ReportsByStatusDto>>
            {
                Success = false,
                Message = "Unable to retrieve statistics",
                Errors = new List<string> { ex.Message }
            };
        }
    }
}
