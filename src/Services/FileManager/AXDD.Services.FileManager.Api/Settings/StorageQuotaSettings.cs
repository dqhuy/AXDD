namespace AXDD.Services.FileManager.Api.Settings;

/// <summary>
/// Storage quota configuration settings
/// </summary>
public class StorageQuotaSettings
{
    /// <summary>
    /// Gets or sets the default quota per enterprise in GB
    /// </summary>
    public long DefaultQuotaPerEnterpriseGB { get; set; } = 100;

    /// <summary>
    /// Gets or sets the warning threshold percentage
    /// </summary>
    public int WarningThresholdPercentage { get; set; } = 80;

    /// <summary>
    /// Gets the default quota in bytes
    /// </summary>
    public long DefaultQuotaBytes => DefaultQuotaPerEnterpriseGB * 1024 * 1024 * 1024;
}
