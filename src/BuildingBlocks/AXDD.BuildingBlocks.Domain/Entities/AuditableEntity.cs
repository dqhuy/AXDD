namespace AXDD.BuildingBlocks.Domain.Entities;

/// <summary>
/// Entity with audit information
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public string? Notes { get; set; }
    public int Version { get; set; }
}
