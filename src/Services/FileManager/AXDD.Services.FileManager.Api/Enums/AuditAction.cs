namespace AXDD.Services.FileManager.Api.Enums;

/// <summary>
/// Audit action type for system logging
/// </summary>
public enum AuditAction
{
    /// <summary>
    /// Create action
    /// </summary>
    Create = 0,

    /// <summary>
    /// Read action
    /// </summary>
    Read = 1,

    /// <summary>
    /// Update action
    /// </summary>
    Update = 2,

    /// <summary>
    /// Delete action
    /// </summary>
    Delete = 3,

    /// <summary>
    /// Download action
    /// </summary>
    Download = 4,

    /// <summary>
    /// Upload action
    /// </summary>
    Upload = 5,

    /// <summary>
    /// Share action
    /// </summary>
    Share = 6,

    /// <summary>
    /// Move action
    /// </summary>
    Move = 7,

    /// <summary>
    /// Approve action
    /// </summary>
    Approve = 8,

    /// <summary>
    /// Reject action
    /// </summary>
    Reject = 9,

    /// <summary>
    /// Login action
    /// </summary>
    Login = 10,

    /// <summary>
    /// Logout action
    /// </summary>
    Logout = 11
}
