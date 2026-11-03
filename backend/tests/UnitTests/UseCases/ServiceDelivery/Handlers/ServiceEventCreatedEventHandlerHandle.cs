using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;
using UseCases.ServiceDelivery.Handlers;
using Core.Common.Constants;

namespace UnitTests.UseCases.ServiceDelivery.Handlers;

public class ServiceEventCreatedEventHandlerHandle
{
    private readonly IRepository<ServiceEventAction> _serviceEventActionRepository = Substitute.For<IRepository<ServiceEventAction>>();
    private readonly ServiceEventCreatedEventHandler _handler;

    public ServiceEventCreatedEventHandlerHandle()
    {
        _handler = new ServiceEventCreatedEventHandler(_serviceEventActionRepository);
    }

    [Fact]
    public async Task RecordsCompletedAction()
    {
        var domainEvent = new ServiceEventCreatedEvent(serviceEventId: 7);

        await _handler.Handle(domainEvent, CancellationToken.None);

        await _serviceEventActionRepository.Received(1).AddAsync(
            Arg.Is<ServiceEventAction>(x =>
                x.ServiceEventId == domainEvent.ServiceEventId &&
                x.ActionTypeId == Constants.ServiceEventActionTypes.UpdateServicePoint &&
                x.StatusId == Constants.ServiceOrderStatuses.Completed &&
                x.Message == null),
            Arg.Any<CancellationToken>());
    }
}
