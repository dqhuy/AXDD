namespace AXDD.BuildingBlocks.Domain.Events;

/// <summary>
/// Base implementation of a domain event
/// </summary>
public abstract record DomainEvent : IDomainEvent
{
    /// <summary>
    /// Initializes a new instance of the DomainEvent class
    /// </summary>
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }

    /// <inheritdoc />
    public DateTime OccurredOn { get; init; }

    /// <inheritdoc />
    public Guid EventId { get; init; }
}
