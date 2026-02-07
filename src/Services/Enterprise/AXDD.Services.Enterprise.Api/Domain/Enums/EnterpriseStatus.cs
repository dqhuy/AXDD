namespace AXDD.Services.Enterprise.Api.Domain.Enums;

/// <summary>
/// Represents the status of an enterprise
/// </summary>
public enum EnterpriseStatus
{
    /// <summary>
    /// Enterprise is under construction
    /// </summary>
    UnderConstruction = 0,

    /// <summary>
    /// Enterprise is active and operating
    /// </summary>
    Active = 1,

    /// <summary>
    /// Enterprise operations are temporarily suspended
    /// </summary>
    Suspended = 2,

    /// <summary>
    /// Enterprise has closed operations
    /// </summary>
    Closed = 3,

    /// <summary>
    /// Enterprise is in liquidation process
    /// </summary>
    Liquidated = 4
}
