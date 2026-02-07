namespace AXDD.Services.FileManager.Api.Enums;

/// <summary>
/// Permission level for folder access
/// </summary>
public enum PermissionLevel
{
    /// <summary>
    /// No access
    /// </summary>
    None = 0,

    /// <summary>
    /// Read access only
    /// </summary>
    Read = 1,

    /// <summary>
    /// Read and write access
    /// </summary>
    Write = 2,

    /// <summary>
    /// Read, write and delete access
    /// </summary>
    Delete = 3,

    /// <summary>
    /// Full administrative access
    /// </summary>
    Admin = 4
}
