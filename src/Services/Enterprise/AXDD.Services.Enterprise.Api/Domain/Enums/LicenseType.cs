namespace AXDD.Services.Enterprise.Api.Domain.Enums;

/// <summary>
/// Represents the type of enterprise license
/// </summary>
public enum LicenseType
{
    /// <summary>
    /// Investment license
    /// </summary>
    Investment = 0,

    /// <summary>
    /// Environmental license
    /// </summary>
    Environment = 1,

    /// <summary>
    /// Construction license
    /// </summary>
    Construction = 2,

    /// <summary>
    /// Fire safety license
    /// </summary>
    FireSafety = 3,

    /// <summary>
    /// Labor license
    /// </summary>
    Labor = 4,

    /// <summary>
    /// Business registration
    /// </summary>
    BusinessRegistration = 5,

    /// <summary>
    /// Other license types
    /// </summary>
    Other = 99
}
