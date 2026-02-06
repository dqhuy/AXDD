using AXDD.BuildingBlocks.Domain.Entities;

namespace AXDD.Services.FileManager.Api.Entities;

/// <summary>
/// Represents storage quota for an enterprise
/// </summary>
public class StorageQuota : BaseEntity
{
    public StorageQuota()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Gets or sets the enterprise code
    /// </summary>
    public string EnterpriseCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total quota in bytes
    /// </summary>
    public long QuotaBytes { get; set; }

    /// <summary>
    /// Gets or sets the used storage in bytes
    /// </summary>
    public long UsedBytes { get; set; }

    /// <summary>
    /// Gets or sets the warning threshold percentage (default 80%)
    /// </summary>
    public int WarningThresholdPercentage { get; set; } = 80;

    /// <summary>
    /// Gets the available bytes
    /// </summary>
    public long AvailableBytes => QuotaBytes - UsedBytes;

    /// <summary>
    /// Gets the usage percentage
    /// </summary>
    public double UsagePercentage => QuotaBytes > 0 ? (double)UsedBytes / QuotaBytes * 100 : 0;

    /// <summary>
    /// Gets whether the quota is exceeded
    /// </summary>
    public bool IsExceeded => UsedBytes >= QuotaBytes;

    /// <summary>
    /// Gets whether the warning threshold is reached
    /// </summary>
    public bool IsWarningThresholdReached => UsagePercentage >= WarningThresholdPercentage;
}
