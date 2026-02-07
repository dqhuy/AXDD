using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a location within a physical storage (shelf, cabinet, etc.)
/// </summary>
public class PhysicalStorageLocation : BaseEntity
{
    public PhysicalStorageLocation()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the physical storage ID
    /// </summary>
    public Guid PhysicalStorageId { get; set; }

    /// <summary>
    /// Gets or sets the physical storage
    /// </summary>
    public PhysicalStorage? PhysicalStorage { get; set; }

    /// <summary>
    /// Gets or sets the unique code of the location
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the location
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the location
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets whether the location is currently occupied
    /// </summary>
    public bool IsOccupied { get; set; }
}
