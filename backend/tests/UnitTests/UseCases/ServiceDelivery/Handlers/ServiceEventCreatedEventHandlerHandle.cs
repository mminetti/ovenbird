using Ardalis.Result;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;
using UseCases.Interfaces.ServiceDelivery;
using UseCases.ServiceDelivery.Handlers;
using Core.Common.Constants;

namespace UnitTests.UseCases.ServiceDelivery.Handlers;

public class ServiceEventCreatedEventHandlerHandle
{
    private readonly IUpdateServicePointFromServiceEventAction _updateServicePointFromServiceEventAction =
        Substitute.For<IUpdateServicePointFromServiceEventAction>();
    private readonly IRepository<ServiceEventAction> _serviceEventActionRepository = Substitute.For<IRepository<ServiceEventAction>>();
    private readonly ServiceEventCreatedEventHandler _handler;

    public ServiceEventCreatedEventHandlerHandle()
    {
        _handler = new ServiceEventCreatedEventHandler(_updateServicePointFromServiceEventAction, _serviceEventActionRepository);
    }

    private static ServiceEventCreatedEvent CreateDomainEvent() => new(serviceEventId: 7);

    [Fact]
    public async Task RecordsCompletedActionWhenUpdateSucceeds()
    {
        _updateServicePointFromServiceEventAction
            .ExecuteAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        var domainEvent = CreateDomainEvent();

        await _handler.Handle(domainEvent, CancellationToken.None);

        await _serviceEventActionRepository.Received(1).AddAsync(
            Arg.Is<ServiceEventAction>(x =>
                x.ServiceEventId == domainEvent.ServiceEventId &&
                x.ActionTypeId == Constants.ServiceEventActionTypes.UpdateServicePoint &&
                x.StatusId == Constants.ServiceOrderStatuses.Completed &&
                x.Message == null),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RecordsErrorActionWithMessageWhenUpdateFails()
    {
        _updateServicePointFromServiceEventAction
            .ExecuteAsync(Arg.Any<long>(), Arg.Any<CancellationToken>())
            .Returns(Result.Invalid(new ValidationError("ServicePoint", "ServicePoint 'SP1' was not found.")));

        var domainEvent = CreateDomainEvent();

        await _handler.Handle(domainEvent, CancellationToken.None);

        await _serviceEventActionRepository.Received(1).AddAsync(
            Arg.Is<ServiceEventAction>(x =>
                x.ServiceEventId == domainEvent.ServiceEventId &&
                x.ActionTypeId == Constants.ServiceEventActionTypes.UpdateServicePoint &&
                x.StatusId == Constants.ServiceOrderStatuses.Error &&
                x.Message == "ServicePoint 'SP1' was not found."),
            Arg.Any<CancellationToken>());
    }
}
