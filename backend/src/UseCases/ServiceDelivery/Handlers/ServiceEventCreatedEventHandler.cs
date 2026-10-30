using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;
using UseCases.Interfaces.ServiceDelivery;

namespace UseCases.ServiceDelivery.Handlers;

public class ServiceEventCreatedEventHandler(
    IUpdateServicePointFromServiceEventAction updateServicePointFromServiceEventAction,
    IRepository<ServiceEventAction> serviceEventActionRepository)
{
    public async Task Handle(ServiceEventCreatedEvent domainEvent, CancellationToken ct)
    {
        var result = await updateServicePointFromServiceEventAction.ExecuteAsync(domainEvent.ServiceEventId, ct);

        await serviceEventActionRepository.AddAsync(new ServiceEventAction
        {
            ServiceEventId = domainEvent.ServiceEventId,
            ActionTypeId = Constants.ServiceEventActionTypes.UpdateServicePoint,
            StatusId = result.IsSuccess ? Constants.ServiceOrderStatuses.Completed : Constants.ServiceOrderStatuses.Error,
            Message = result.IsSuccess ? null : string.Join("; ", result.ValidationErrors.Select(e => e.ErrorMessage)),
        }, ct);

        // TODO: send notification (Constants.ServiceEventActionTypes.SendNotification) once the generic
        // notification template service exists - tracked as a separate task.
    }
}
