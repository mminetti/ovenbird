using Core.Common.Constants;
using Core.ServiceDelivery.Events;

namespace Core.ServiceDelivery.Handlers;

public class ServiceStartScheduledEventHandler
{
    public Task Handle(ServiceStartScheduledEvent domainEvent, CancellationToken ct)
    {
        domainEvent.ServiceStart.ServiceDate = domainEvent.ServiceDate;
        domainEvent.ServiceStart.ServiceOrderStatusId = Constants.ServiceOrderStatuses.Scheduled;

        // TODO: use domainEvent.MarketDocumentItemId to drive related logic once requirements are defined.
        return Task.CompletedTask;
    }
}
