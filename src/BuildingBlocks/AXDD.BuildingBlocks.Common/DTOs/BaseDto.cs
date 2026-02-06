namespace AXDD.BuildingBlocks.Common.DTOs;

/// <summary>
/// Base DTO for all data transfer objects
/// </summary>
public abstract class BaseDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
