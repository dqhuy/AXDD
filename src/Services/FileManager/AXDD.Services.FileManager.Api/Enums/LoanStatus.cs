namespace AXDD.Services.FileManager.Api.Enums;

/// <summary>
/// Document loan status
/// </summary>
public enum LoanStatus
{
    /// <summary>
    /// Pending approval
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Approved
    /// </summary>
    Approved = 1,

    /// <summary>
    /// Rejected
    /// </summary>
    Rejected = 2,

    /// <summary>
    /// Currently borrowed
    /// </summary>
    Borrowed = 3,

    /// <summary>
    /// Returned
    /// </summary>
    Returned = 4,

    /// <summary>
    /// Overdue
    /// </summary>
    Overdue = 5
}
