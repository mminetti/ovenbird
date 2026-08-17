using Wolverine;

namespace Infrastructure.Data;

/// <summary>
/// Dispatches domain events via Wolverine's IMessageBus (PublishAsync).
/// Replaces Ardalis.SharedKernel.MediatorDomainEventDispatcher which requires Mediator.IMediator.
/// </summary>
public class WolverineDomainEventDispatcher(IMessageBus bus) : IDomainEventDispatcher
{
    public async Task DispatchAndClearEvents(IEnumerable<IHasDomainEvents> entitiesWithEvents)
    {
        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.ClearDomainEvents();
            foreach (var domainEvent in events)
            {
                await bus.PublishAsync(domainEvent);
            }
        }
    }
}
