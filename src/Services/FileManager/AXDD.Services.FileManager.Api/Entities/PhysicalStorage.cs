using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a physical storage location (physical document archive)
/// </summary>
public class PhysicalStorage : BaseEntity
{
    public PhysicalStorage()
    {
        Id = Guid.NewGuid();
        Locations = new List<PhysicalStorageLocation>();
    }

    /// <summary>
    /// Gets or sets the unique code of the physical storage
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the physical storage
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the physical storage
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the address of the physical storage
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets whether the physical storage is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the enterprise code this storage belongs to
    /// </summary>
    public string EnterpriseCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of locations within this physical storage
    /// </summary>
    public ICollection<PhysicalStorageLocation> Locations { get; set; }
}
