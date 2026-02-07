using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.MasterData.Api.Entities;

/// <summary>
/// Represents a configuration setting
/// </summary>
public class Configuration : BaseEntity
{
    /// <summary>
    /// Gets or sets the configuration key
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the configuration value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the category
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the data type (String, Int, Boolean, Json, etc.)
    /// </summary>
    public string DataType { get; set; } = "String";

    /// <summary>
    /// Gets or sets whether this configuration is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this configuration is system-level (cannot be deleted)
    /// </summary>
    public bool IsSystem { get; set; }
}
