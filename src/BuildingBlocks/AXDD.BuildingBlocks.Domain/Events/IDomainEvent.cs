namespace AXDD.BuildingBlocks.Domain.Events;

/// <summary>
/// Base interface for domain events
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the date and time when the event occurred
    /// </summary>
    DateTime OccurredOn { get; }

    /// <summary>
    /// Gets the unique identifier for this event
    /// </summary>
    Guid EventId { get; }
}
