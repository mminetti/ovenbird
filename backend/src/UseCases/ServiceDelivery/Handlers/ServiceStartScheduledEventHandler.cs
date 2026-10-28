using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;
using UseCases.Interfaces.ServiceDelivery;

namespace UseCases.ServiceDelivery.Handlers;

public class ServiceStartScheduledEventHandler(
    IUpdateServicePointAction updateServicePointAction,
    IRepository<ServiceEventAction> serviceEventActionRepository)
{
    public async Task Handle(ServiceStartScheduledEvent domainEvent, CancellationToken ct)
    {
        var result = await updateServicePointAction.ExecuteAsync(domainEvent, ct);

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
