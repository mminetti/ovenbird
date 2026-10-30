namespace Core.ServiceDelivery.Events;

public sealed class ServiceEventCreatedEvent(long serviceEventId) : DomainEventBase
{
    public long ServiceEventId { get; set; } = serviceEventId;
}
