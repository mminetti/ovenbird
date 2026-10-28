using Core.ServiceDelivery.Events;

namespace UseCases.Interfaces.ServiceDelivery;

public interface IUpdateServicePointAction
{
    Task<Result> ExecuteAsync(ServiceStartScheduledEvent domainEvent, CancellationToken ct);
}
