using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a security level for documents and folders
/// </summary>
public class SecurityLevel : BaseEntity
{
    public SecurityLevel()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the unique code of the security level
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the security level
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the security level
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the level value (1 = Public, 2 = Internal, 3 = Confidential, 4 = Restricted)
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets whether access to items with this security level requires approval
    /// </summary>
    public bool RequiresApproval { get; set; }

    /// <summary>
    /// Gets or sets whether the security level is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}
