using Ardalis.Result;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;
using UseCases.Interfaces.ServiceDelivery;
using UseCases.ServiceDelivery.Handlers;
using Core.Common.Constants;

namespace UnitTests.UseCases.ServiceDelivery.Handlers;

public class ServiceStartScheduledEventHandlerHandle
{
    private readonly IUpdateServicePointAction _updateServicePointAction = Substitute.For<IUpdateServicePointAction>();
    private readonly IRepository<ServiceEventAction> _serviceEventActionRepository = Substitute.For<IRepository<ServiceEventAction>>();
    private readonly ServiceStartScheduledEventHandler _handler;

    public ServiceStartScheduledEventHandlerHandle()
    {
        _handler = new ServiceStartScheduledEventHandler(_updateServicePointAction, _serviceEventActionRepository);
    }

    private static ServiceStartScheduledEvent CreateDomainEvent() => new(
        serviceEventId: 7,
        servicePointIdentifier: "SP1",
        commodityCode: "E",
        customerClassCode: "RESIDENTIAL",
        loadProfileCode: "PROFILE",
        readCycleCode: "14",
        advanceMeterInfoCode: "AMSR",
        meterNumber: "M1",
        premiseAddressLine1: "123 Main St",
        premiseCity: "Austin",
        premiseState: "TX",
        premisePostalCode: "787010000",
        countyName: "TRAVIS");

    [Fact]
    public async Task RecordsCompletedActionWhenUpdateSucceeds()
    {
        _updateServicePointAction
            .ExecuteAsync(Arg.Any<ServiceStartScheduledEvent>(), Arg.Any<CancellationToken>())
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
        _updateServicePointAction
            .ExecuteAsync(Arg.Any<ServiceStartScheduledEvent>(), Arg.Any<CancellationToken>())
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
