namespace AXDD.BuildingBlocks.Domain.Events;

/// <summary>
/// Handler interface for domain events
/// </summary>
/// <typeparam name="TDomainEvent">Type of domain event to handle</typeparam>
public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    /// <summary>
    /// Handles the domain event
    /// </summary>
    /// <param name="domainEvent">The domain event to handle</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
