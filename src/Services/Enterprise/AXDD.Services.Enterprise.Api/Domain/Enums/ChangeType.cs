namespace AXDD.Services.Enterprise.Api.Domain.Enums;

/// <summary>
/// Represents the type of change in enterprise history
/// </summary>
public enum ChangeType
{
    /// <summary>
    /// Enterprise was created
    /// </summary>
    Created = 0,

    /// <summary>
    /// Enterprise information was updated
    /// </summary>
    Updated = 1,

    /// <summary>
    /// Enterprise status was changed
    /// </summary>
    StatusChanged = 2,

    /// <summary>
    /// License was added
    /// </summary>
    LicenseAdded = 3,

    /// <summary>
    /// License was updated
    /// </summary>
    LicenseUpdated = 4,

    /// <summary>
    /// License was removed
    /// </summary>
    LicenseRemoved = 5,

    /// <summary>
    /// Contact person was added
    /// </summary>
    ContactAdded = 6,

    /// <summary>
    /// Contact person was updated
    /// </summary>
    ContactUpdated = 7,

    /// <summary>
    /// Contact person was removed
    /// </summary>
    ContactRemoved = 8,

    /// <summary>
    /// Main contact was changed
    /// </summary>
    MainContactChanged = 9,

    /// <summary>
    /// Enterprise was deleted
    /// </summary>
    Deleted = 10
}
