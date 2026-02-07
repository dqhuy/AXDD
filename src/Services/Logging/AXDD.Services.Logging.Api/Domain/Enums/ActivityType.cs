namespace AXDD.Services.Logging.Api.Domain.Enums;

/// <summary>
/// Defines types of user activities
/// </summary>
public enum ActivityType
{
    /// <summary>
    /// User logged in
    /// </summary>
    Login = 0,

    /// <summary>
    /// User logged out
    /// </summary>
    Logout = 1,

    /// <summary>
    /// User created a resource
    /// </summary>
    Create = 2,

    /// <summary>
    /// User updated a resource
    /// </summary>
    Update = 3,

    /// <summary>
    /// User deleted a resource
    /// </summary>
    Delete = 4,

    /// <summary>
    /// User viewed a resource
    /// </summary>
    View = 5,

    /// <summary>
    /// User performed a search
    /// </summary>
    Search = 6,

    /// <summary>
    /// User downloaded content
    /// </summary>
    Download = 7,

    /// <summary>
    /// User uploaded content
    /// </summary>
    Upload = 8,

    /// <summary>
    /// User exported data
    /// </summary>
    Export = 9,

    /// <summary>
    /// User imported data
    /// </summary>
    Import = 10
}
