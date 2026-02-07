using AXDD.WebApp.Admin.Models.ApiModels;

namespace AXDD.WebApp.Admin.Services;

/// <summary>
/// Interface for authentication API service
/// </summary>
public interface IAuthApiService
{
    /// <summary>
    /// Login user
    /// </summary>
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refresh token
    /// </summary>
    Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate token
    /// </summary>
    Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default);
}

/// <summary>
/// Authentication API service implementation
/// </summary>
public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuthApiService> _logger;

    public AuthApiService(HttpClient httpClient, ILogger<AuthApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync<LoginRequest, ApiResponse<LoginResponse>>(
                "/api/v1/auth/login",
                request,
                cancellationToken);

            return response ?? new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error during login request");
            return new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "Unable to connect to authentication service",
                Errors = new List<string> { ex.Message }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login");
            return new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "An unexpected error occurred",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync<object, ApiResponse<LoginResponse>>(
                "/api/v1/auth/refresh",
                new { RefreshToken = refreshToken },
                cancellationToken);

            return response ?? new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "No response from server"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token");
            return new ApiResponse<LoginResponse>
            {
                Success = false,
                Message = "Unable to refresh token",
                Errors = new List<string> { ex.Message }
            };
        }
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        try
        {
            _httpClient.AddBearerToken(token);
            var response = await _httpClient.GetAsync("/api/v1/auth/validate", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating token");
            return false;
        }
    }
}
