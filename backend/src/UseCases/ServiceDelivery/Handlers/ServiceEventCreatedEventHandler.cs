using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;

namespace UseCases.ServiceDelivery.Handlers;

public class ServiceEventCreatedEventHandler(IRepository<ServiceEventAction> serviceEventActionRepository)
{
    public Task Handle(ServiceEventCreatedEvent domainEvent, CancellationToken ct) =>
        // TODO: send notification (Constants.ServiceEventActionTypes.SendNotification) once the generic
        // notification template service exists - tracked as a separate task.
        serviceEventActionRepository.AddAsync(new ServiceEventAction
        {
            ServiceEventId = domainEvent.ServiceEventId,
            ActionTypeId = Constants.ServiceEventActionTypes.UpdateServicePoint,
            StatusId = Constants.ServiceOrderStatuses.Completed,
        }, ct);
}
