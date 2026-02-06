using AXDD.BuildingBlocks.Common.Results;
using AXDD.Services.Auth.Api.Data;
using AXDD.Services.Auth.Api.DTOs;
using AXDD.Services.Auth.Api.Entities;
using AXDD.Services.Auth.Api.Services.Interfaces;
using AXDD.Services.Auth.Api.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AXDD.Services.Auth.Api.Services.Implementations;

/// <summary>
/// Authentication service implementation
/// </summary>
public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly AuthDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        AuthDbContext context,
        IJwtService jwtService,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _jwtService = jwtService;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Result<LoginResponse>> LoginAsync(
        LoginRequest request,
        string? ipAddress,
        string? userAgent,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Find user by username or email
        var user = await _userManager.FindByNameAsync(request.Username)
            ?? await _userManager.FindByEmailAsync(request.Username);

        if (user == null)
        {
            _logger.LogWarning("Login failed: User not found - {Username}", request.Username);
            return Result<LoginResponse>.Failure("Invalid username or password");
        }

        // Check if user is active
        if (!user.IsActive)
        {
            _logger.LogWarning("Login failed: User inactive - {UserId}", user.Id);
            return Result<LoginResponse>.Failure("User account is inactive");
        }

        // Check password
        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
            {
                _logger.LogWarning("Login failed: Account locked out - {UserId}", user.Id);
                return Result<LoginResponse>.Failure("Account is locked out");
            }

            if (result.IsNotAllowed)
            {
                _logger.LogWarning("Login failed: Not allowed - {UserId}", user.Id);
                return Result<LoginResponse>.Failure("Login is not allowed");
            }

            _logger.LogWarning("Login failed: Invalid password - {UserId}", user.Id);
            return Result<LoginResponse>.Failure("Invalid username or password");
        }

        // Generate tokens
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var refreshToken = _jwtService.GenerateRefreshToken();

        // Save refresh token
        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedByIp = ipAddress
        };

        _context.RefreshTokens.Add(refreshTokenEntity);

        // Create user session
        var session = new UserSession
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            SessionToken = Guid.NewGuid().ToString(),
            IpAddress = ipAddress,
            UserAgent = userAgent,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        };

        _context.UserSessions.Add(session);

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User logged in successfully - {UserId}", user.Id);

        var response = new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
            User = new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt,
                Roles = roles.ToList()
            }
        };

        return Result<LoginResponse>.Success(response);
    }

    /// <inheritdoc />
    public async Task<Result<UserDto>> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Check if username already exists
        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser != null)
        {
            return Result<UserDto>.Failure("Username is already taken");
        }

        // Check if email already exists
        existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<UserDto>.Failure("Email is already registered");
        }

        // Create new user
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            UserName = request.Username,
            Email = request.Email,
            FullName = request.FullName,
            PhoneNumber = request.PhoneNumber,
            EmailConfirmed = false,
            PhoneNumberConfirmed = false,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("User registration failed - {Errors}", string.Join(", ", errors));
            return Result<UserDto>.Failure(errors);
        }

        _logger.LogInformation("User registered successfully - {UserId}", user.Id);

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            Roles = []
        };

        return Result<UserDto>.Success(userDto);
    }

    /// <inheritdoc />
    public async Task<Result<LoginResponse>> RefreshTokenAsync(
        string refreshToken,
        string? ipAddress,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        // Find the refresh token
        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (token == null)
        {
            _logger.LogWarning("Refresh token not found");
            return Result<LoginResponse>.Failure("Invalid refresh token");
        }

        // Check if token is active
        if (!token.IsActive)
        {
            _logger.LogWarning("Refresh token is not active - {TokenId}", token.Id);
            return Result<LoginResponse>.Failure("Refresh token is expired or revoked");
        }

        // Check if user is active
        if (token.User == null || !token.User.IsActive)
        {
            _logger.LogWarning("User is inactive - {UserId}", token.UserId);
            return Result<LoginResponse>.Failure("User account is inactive");
        }

        // Generate new tokens
        var roles = await _userManager.GetRolesAsync(token.User);
        var newAccessToken = _jwtService.GenerateAccessToken(token.User, roles);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Revoke old refresh token
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;
        token.ReplacedByToken = newRefreshToken;

        // Save new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = token.UserId,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays),
            CreatedByIp = ipAddress
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Token refreshed successfully - {UserId}", token.UserId);

        var response = new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresIn = _jwtSettings.AccessTokenExpirationMinutes * 60,
            User = new UserDto
            {
                Id = token.User.Id,
                Username = token.User.UserName ?? string.Empty,
                Email = token.User.Email ?? string.Empty,
                FullName = token.User.FullName,
                PhoneNumber = token.User.PhoneNumber,
                EmailConfirmed = token.User.EmailConfirmed,
                PhoneNumberConfirmed = token.User.PhoneNumberConfirmed,
                IsActive = token.User.IsActive,
                LastLoginAt = token.User.LastLoginAt,
                CreatedAt = token.User.CreatedAt,
                Roles = roles.ToList()
            }
        };

        return Result<LoginResponse>.Success(response);
    }

    /// <inheritdoc />
    public async Task<Result> RevokeTokenAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (token == null)
        {
            _logger.LogWarning("Refresh token not found for revocation");
            return Result.Failure("Invalid refresh token");
        }

        if (!token.IsActive)
        {
            _logger.LogWarning("Refresh token is already inactive - {TokenId}", token.Id);
            return Result.Failure("Token is already revoked or expired");
        }

        // Revoke token
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        token.RevokedByIp = ipAddress;

        // End active sessions for this user
        var activeSessions = await _context.UserSessions
            .Where(s => s.UserId == token.UserId && s.IsActive)
            .ToListAsync(cancellationToken);

        foreach (var session in activeSessions)
        {
            session.IsActive = false;
            session.EndedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Token revoked successfully - {UserId}", token.UserId);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> ChangePasswordAsync(
        Guid userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Password change failed - {UserId}: {Errors}", userId, string.Join(", ", errors));
            return Result.Failure(errors);
        }

        _logger.LogInformation("Password changed successfully - {UserId}", userId);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            // Don't reveal that the user doesn't exist
            _logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
            return Result.Success();
        }

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        // TODO: Send reset token via email
        // For now, just log it (in production, send email)
        _logger.LogInformation("Password reset token generated for {UserId}: {Token}", user.Id, resetToken);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            return Result.Failure("Invalid reset request");
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description).ToList();
            _logger.LogWarning("Password reset failed - {UserId}: {Errors}", user.Id, string.Join(", ", errors));
            return Result.Failure(errors);
        }

        _logger.LogInformation("Password reset successfully - {UserId}", user.Id);

        return Result.Success();
    }

    /// <inheritdoc />
    public async Task<Result<UserDto>> GetUserInfoAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            return Result<UserDto>.Failure("User not found");
        }

        var roles = await _userManager.GetRolesAsync(user);

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.UserName ?? string.Empty,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            Roles = roles.ToList()
        };

        return Result<UserDto>.Success(userDto);
    }
}
