namespace AXDD.Services.FileManager.Api.Enums;

/// <summary>
/// Document loan type
/// </summary>
public enum LoanType
{
    /// <summary>
    /// Borrow hard copy
    /// </summary>
    HardCopy = 0,

    /// <summary>
    /// Borrow copy
    /// </summary>
    Copy = 1,

    /// <summary>
    /// Borrow backup
    /// </summary>
    Backup = 2,

    /// <summary>
    /// Read only
    /// </summary>
    ReadOnly = 3,

    /// <summary>
    /// Certified copy (Sao y)
    /// </summary>
    CertifiedCopy = 4
}
