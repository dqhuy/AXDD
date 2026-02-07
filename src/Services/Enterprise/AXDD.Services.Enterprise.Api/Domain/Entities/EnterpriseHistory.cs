using AXDD.BuildingBlocks.Domain.Entities;
using AXDD.Services.Enterprise.Api.Domain.Enums;

namespace AXDD.Services.Enterprise.Api.Domain.Entities;

/// <summary>
/// Represents a historical change record for an enterprise
/// </summary>
public class EnterpriseHistory : BaseEntity
{
    /// <summary>
    /// Gets or sets the enterprise ID
    /// </summary>
    public Guid EnterpriseId { get; set; }

    /// <summary>
    /// Gets or sets when the change occurred
    /// </summary>
    public DateTime ChangedAt { get; set; }

    /// <summary>
    /// Gets or sets who made the change
    /// </summary>
    public string? ChangedBy { get; set; }

    /// <summary>
    /// Gets or sets the type of change
    /// </summary>
    public ChangeType ChangeType { get; set; }

    /// <summary>
    /// Gets or sets the field that was changed
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// Gets or sets the old value
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// Gets or sets the new value
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// Gets or sets the reason for the change
    /// </summary>
    public string? Reason { get; set; }

    /// <summary>
    /// Gets or sets additional details
    /// </summary>
    public string? Details { get; set; }

    // Navigation properties

    /// <summary>
    /// Gets or sets the enterprise
    /// </summary>
    public EnterpriseEntity? Enterprise { get; set; }
}
