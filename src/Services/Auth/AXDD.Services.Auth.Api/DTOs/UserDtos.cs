namespace AXDD.Services.Auth.Api.DTOs;

/// <summary>
/// User DTO
/// </summary>
public class UserDto
{
    /// <summary>
    /// Gets or sets the user identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets whether the email is confirmed
    /// </summary>
    public bool EmailConfirmed { get; set; }

    /// <summary>
    /// Gets or sets whether the phone number is confirmed
    /// </summary>
    public bool PhoneNumberConfirmed { get; set; }

    /// <summary>
    /// Gets or sets whether the user is active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Gets or sets the last login timestamp
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Gets or sets when the user was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the user's roles
    /// </summary>
    public List<string> Roles { get; set; } = [];
}

/// <summary>
/// Create user request DTO
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Gets or sets the username
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the full name
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets whether the user is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the role names to assign
    /// </summary>
    public List<string> Roles { get; set; } = [];
}

/// <summary>
/// Update user request DTO
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// Gets or sets the email
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the full name
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// Gets or sets the phone number
    /// </summary>
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets whether the user is active
    /// </summary>
    public bool? IsActive { get; set; }
}

/// <summary>
/// Assign roles request DTO
/// </summary>
public class AssignRolesRequest
{
    /// <summary>
    /// Gets or sets the role names to assign
    /// </summary>
    public List<string> Roles { get; set; } = [];
}
