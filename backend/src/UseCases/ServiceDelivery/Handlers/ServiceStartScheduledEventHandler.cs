using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;
using UseCases.Interfaces.ServiceDelivery;
using CoreConstants = Core.Common.Constants.Constants;

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
            ActionTypeId = CoreConstants.ServiceEventActionTypes.UpdateServicePoint,
            StatusId = result.IsSuccess ? CoreConstants.ServiceOrderStatuses.Completed : CoreConstants.ServiceOrderStatuses.Error,
            Message = result.IsSuccess ? null : string.Join("; ", result.ValidationErrors.Select(e => e.ErrorMessage)),
        }, ct);

        // TODO: send notification (Constants.ServiceEventActionTypes.SendNotification) once the generic
        // notification template service exists - tracked as a separate task.
    }
}
