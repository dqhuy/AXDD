using System.Security.Claims;
using AXDD.BuildingBlocks.Common.DTOs;
using AXDD.Services.Auth.Api.DTOs;
using AXDD.Services.Auth.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Auth.Api.Controllers;

/// <summary>
/// Authentication controller
/// </summary>
[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login with username and password
    /// </summary>
    /// <param name="request">Login request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Login response with access and refresh tokens</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.ValidationError(
                ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                )
            ));
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

        var result = await _authService.LoginAsync(request, ipAddress, userAgent, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Failure(result.Error ?? "Login failed", result.Errors?.ToList()));
        }

        return Ok(ApiResponse<LoginResponse>.Success(result.Value!, "Login successful"));
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="request">Registration request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created user</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.ValidationError(
                ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                )
            ));
        }

        var result = await _authService.RegisterAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Failure(result.Error ?? "Registration failed", result.Errors?.ToList()));
        }

        return CreatedAtAction(nameof(GetUserInfoAsync), null, ApiResponse<UserDto>.Success(result.Value!, "Registration successful", 201));
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>New access and refresh tokens</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.ValidationError(
                ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                )
            ));
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await _authService.RefreshTokenAsync(request.RefreshToken, ipAddress, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Failure(result.Error ?? "Token refresh failed", result.Errors?.ToList()));
        }

        return Ok(ApiResponse<LoginResponse>.Success(result.Value!, "Token refreshed successfully"));
    }

    /// <summary>
    /// Logout (revoke refresh token)
    /// </summary>
    /// <param name="request">Refresh token request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> LogoutAsync([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.ValidationError(
                ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                )
            ));
        }

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

        var result = await _authService.RevokeTokenAsync(request.RefreshToken, ipAddress, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Failure(result.Error ?? "Logout failed", result.Errors?.ToList()));
        }

        return Ok(ApiResponse.Success("Logout successful"));
    }

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    /// <param name="request">Change password request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePasswordAsync([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.ValidationError(
                ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                )
            ));
        }

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(ApiResponse<object>.Failure("Invalid user", null, 401));
        }

        var result = await _authService.ChangePasswordAsync(userId, request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Failure(result.Error ?? "Password change failed", result.Errors?.ToList()));
        }

        return Ok(ApiResponse.Success("Password changed successfully"));
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    /// <param name="request">Forgot password request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.ValidationError(
                ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                )
            ));
        }

        var result = await _authService.ForgotPasswordAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Failure(result.Error ?? "Request failed", result.Errors?.ToList()));
        }

        return Ok(ApiResponse.Success("Password reset instructions have been sent to your email"));
    }

    /// <summary>
    /// Reset password using reset token
    /// </summary>
    /// <param name="request">Reset password request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success result</returns>
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse<object>.ValidationError(
                ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                )
            ));
        }

        var result = await _authService.ResetPasswordAsync(request, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(ApiResponse<object>.Failure(result.Error ?? "Password reset failed", result.Errors?.ToList()));
        }

        return Ok(ApiResponse.Success("Password reset successfully"));
    }

    /// <summary>
    /// Get current user information
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User information</returns>
    [HttpGet("user-info")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserInfoAsync(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            return Unauthorized(ApiResponse<object>.Failure("Invalid user", null, 401));
        }

        var result = await _authService.GetUserInfoAsync(userId, cancellationToken);

        if (result.IsFailure)
        {
            return NotFound(ApiResponse<object>.NotFound(result.Error ?? "User not found"));
        }

        return Ok(ApiResponse<UserDto>.Success(result.Value!));
    }
}
