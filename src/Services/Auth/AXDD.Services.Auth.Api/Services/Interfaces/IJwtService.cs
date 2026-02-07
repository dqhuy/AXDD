using System.Security.Claims;
using AXDD.Services.Auth.Api.Entities;

namespace AXDD.Services.Auth.Api.Services.Interfaces;

/// <summary>
/// JWT token service interface
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Generates an access token (JWT) for the user
    /// </summary>
    /// <param name="user">The user</param>
    /// <param name="roles">The user's roles</param>
    /// <returns>The generated JWT token</returns>
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);

    /// <summary>
    /// Generates a refresh token
    /// </summary>
    /// <returns>The generated refresh token</returns>
    string GenerateRefreshToken();

    /// <summary>
    /// Validates a JWT token
    /// </summary>
    /// <param name="token">The token to validate</param>
    /// <returns>The claims principal if valid, null otherwise</returns>
    ClaimsPrincipal? ValidateToken(string token);

    /// <summary>
    /// Gets the user ID from a JWT token
    /// </summary>
    /// <param name="token">The token</param>
    /// <returns>The user ID if found, null otherwise</returns>
    Guid? GetUserIdFromToken(string token);
}
