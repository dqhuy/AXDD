namespace AXDD.Services.Auth.Api.DTOs;

/// <summary>
/// Role DTO
/// </summary>
public class RoleDto
{
    /// <summary>
    /// Gets or sets the role identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the role name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets when the role was created
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Create role request DTO
/// </summary>
public class CreateRoleRequest
{
    /// <summary>
    /// Gets or sets the role name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the role description
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// Update role request DTO
/// </summary>
public class UpdateRoleRequest
{
    /// <summary>
    /// Gets or sets the role name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the role description
    /// </summary>
    public string? Description { get; set; }
}
