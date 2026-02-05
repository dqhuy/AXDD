using AXDD.BuildingBlocks.Common.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace AXDD.Services.Auth.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation("Login attempt for user: {Username}", request.Username);
        
        var response = new
        {
            Token = "sample-jwt-token",
            Username = request.Username,
            ExpiresIn = 3600
        };

        return Ok(ApiResponse<object>.SuccessResponse(response, "Login successful"));
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        _logger.LogInformation("Registration attempt for user: {Username}", request.Username);
        
        return Ok(ApiResponse<object>.SuccessResponse(
            new { UserId = Guid.NewGuid(), Username = request.Username },
            "Registration successful"
        ));
    }
}

public record LoginRequest(string Username, string Password);
public record RegisterRequest(string Username, string Password, string Email);
