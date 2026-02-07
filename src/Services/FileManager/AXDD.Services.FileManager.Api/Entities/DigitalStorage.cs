using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents a digital storage (electronic document repository)
/// </summary>
public class DigitalStorage : BaseEntity
{
    public DigitalStorage()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the unique code of the digital storage
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the digital storage
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the digital storage
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the MinIO bucket name
    /// </summary>
    public string BucketName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total capacity in bytes
    /// </summary>
    public long TotalCapacityBytes { get; set; }

    /// <summary>
    /// Gets or sets the used capacity in bytes
    /// </summary>
    public long UsedCapacityBytes { get; set; }

    /// <summary>
    /// Gets or sets whether the digital storage is active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the enterprise code this storage belongs to
    /// </summary>
    public string EnterpriseCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets the available capacity in bytes
    /// </summary>
    public long AvailableCapacityBytes => TotalCapacityBytes - UsedCapacityBytes;

    /// <summary>
    /// Gets the usage percentage
    /// </summary>
    public double UsagePercentage => TotalCapacityBytes > 0 
        ? (double)UsedCapacityBytes / TotalCapacityBytes * 100 
        : 0;
}
