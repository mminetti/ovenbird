using Core.Common;
using Core.ServiceDelivery.Events;

namespace Core.ServiceDelivery;

public class ServiceEvent : AuditableEntityBase<long>
{
    public int EventTypeId { get; set; }
    public int ReferenceTypeId { get; set; }
    public long ReferenceId { get; set; }

    public ServiceEventType EventType { get; set; } = default!;
    public ServiceEventReferenceType ReferenceType { get; set; } = default!;
    public ICollection<ServiceEventAction> Actions { get; set; } = [];

    public void MarkCreated() => RegisterDomainEvent(new ServiceEventCreatedEvent(Id));
}
