using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.MasterData.Api.Entities;

/// <summary>
/// Represents an industry code according to VSIC (Vietnamese Standard Industrial Classification)
/// </summary>
public class IndustryCode : BaseEntity
{
    /// <summary>
    /// Gets or sets the industry code (e.g., "C", "C10", "C1011")
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the industry name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the parent code (null for top-level)
    /// </summary>
    public string? ParentCode { get; set; }

    /// <summary>
    /// Gets or sets the level (1 = Section, 2 = Division, 3 = Group, 4 = Class)
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Gets or sets whether this code is active
    /// </summary>
    public bool IsActive { get; set; } = true;
}
