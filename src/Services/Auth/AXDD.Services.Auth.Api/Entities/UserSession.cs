namespace AXDD.Services.Auth.Api.Entities;

/// <summary>
/// User session entity for tracking user login sessions
/// </summary>
public class UserSession
{
    /// <summary>
    /// Gets or sets the unique identifier
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the user identifier
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the session token
    /// </summary>
    public string SessionToken { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the IP address
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// Gets or sets the user agent string
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Gets or sets when the session was created
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets when the session expires
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets when the session was last accessed
    /// </summary>
    public DateTime? LastAccessedAt { get; set; }

    /// <summary>
    /// Gets or sets whether the session is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets when the session was ended
    /// </summary>
    public DateTime? EndedAt { get; set; }

    /// <summary>
    /// Navigation property to the user
    /// </summary>
    public virtual ApplicationUser? User { get; set; }

    /// <summary>
    /// Check if the session is expired
    /// </summary>
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
