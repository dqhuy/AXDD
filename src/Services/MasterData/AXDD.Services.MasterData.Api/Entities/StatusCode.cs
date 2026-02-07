using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.MasterData.Api.Entities;

/// <summary>
/// Represents a status code for various entities
/// </summary>
public class StatusCode : BaseEntity
{
    /// <summary>
    /// Gets or sets the status code
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the status name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the entity type this status applies to (Enterprise, Project, Report, etc.)
    /// </summary>
    public string EntityType { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the color for UI display (hex color code)
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Gets or sets whether this is a final status
    /// </summary>
    public bool IsFinal { get; set; }

    /// <summary>
    /// Gets or sets whether this status is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the display order
    /// </summary>
    public int DisplayOrder { get; set; }
}
