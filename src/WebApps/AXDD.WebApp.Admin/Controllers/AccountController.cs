using AXDD.WebApp.Admin.Models.ApiModels;
using AXDD.WebApp.Admin.Models.ViewModels;
using AXDD.WebApp.Admin.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AXDD.WebApp.Admin.Controllers;

/// <summary>
/// Controller for authentication actions
/// </summary>
public class AccountController : Controller
{
    private readonly IAuthApiService _authApiService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAuthApiService authApiService, ILogger<AccountController> logger)
    {
        _authApiService = authApiService;
        _logger = logger;
    }

    /// <summary>
    /// Display login page
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        var model = new LoginViewModel
        {
            ReturnUrl = returnUrl
        };

        return View(model);
    }

    /// <summary>
    /// Process login request
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var request = new LoginRequest
            {
                Username = model.Username,
                Password = model.Password
            };

            var response = await _authApiService.LoginAsync(request, cancellationToken);

            if (!response.Success || response.Data == null)
            {
                ModelState.AddModelError(string.Empty, response.Message ?? "Invalid username or password");
                return View(model);
            }

            var loginData = response.Data;

            // Parse JWT token to get claims
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(loginData.Token);

            // Create claims for cookie authentication
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginData.User.Id.ToString()),
                new Claim(ClaimTypes.Name, loginData.User.Username),
                new Claim(ClaimTypes.Email, loginData.User.Email),
                new Claim("FullName", loginData.User.FullName)
            };

            // Add roles
            foreach (var role in loginData.User.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = loginData.ExpiresAt,
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Store JWT token in cookie for API calls
            Response.Cookies.Append("AuthToken", loginData.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = HttpContext.Request.IsHttps, // Use HTTPS only in production
                SameSite = SameSiteMode.Strict,
                Expires = loginData.ExpiresAt
            });

            // Store refresh token
            Response.Cookies.Append("RefreshToken", loginData.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = HttpContext.Request.IsHttps, // Use HTTPS only in production
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });

            _logger.LogInformation("User {Username} logged in successfully", model.Username);

            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for user {Username}", model.Username);
            ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
            return View(model);
        }
    }

    /// <summary>
    /// Logout user
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Remove JWT tokens from cookies
            Response.Cookies.Delete("AuthToken");
            Response.Cookies.Delete("RefreshToken");

            _logger.LogInformation("User logged out successfully");

            return RedirectToAction("Login");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return RedirectToAction("Login");
        }
    }

    /// <summary>
    /// Access denied page
    /// </summary>
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
