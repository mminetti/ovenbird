using Core.Common.Constants;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;
using Core.ServiceDelivery.Handlers;
using Core.ServiceDelivery.Specifications;

namespace UnitTests.Core.ServiceDelivery.Handlers;

public class ServiceStartScheduledEventHandlerHandle
{
    private readonly IRepository<ServiceStart> _serviceStartRepository = Substitute.For<IRepository<ServiceStart>>();
    private readonly IRepository<ServicePoint> _servicePointRepository = Substitute.For<IRepository<ServicePoint>>();
    private readonly ServiceStartScheduledEventHandler _handler;

    public ServiceStartScheduledEventHandlerHandle()
    {
        _handler = new ServiceStartScheduledEventHandler(_serviceStartRepository, _servicePointRepository);
    }

    [Fact]
    public async Task SchedulesServiceStartAndUpdatesServicePointAndPremise()
    {
        var serviceStart = new ServiceStart { Id = 99 };
        var premise = new Premise();
        var servicePoint = new ServicePoint { Id = 7, Identifier = "SP1", CommodityId = 2, Premise = premise };

        _serviceStartRepository.GetByIdAsync(serviceStart.Id, Arg.Any<CancellationToken>())
            .Returns(serviceStart);

        _servicePointRepository
            .FirstOrDefaultAsync(Arg.Any<ServicePointByIdentifierAndCommoditySpec>(), Arg.Any<CancellationToken>())
            .Returns(servicePoint);

        var serviceDate = new DateOnly(2025, 11, 12);

        var domainEvent = new ServiceStartScheduledEvent(
            serviceStartId: serviceStart.Id,
            serviceDate: serviceDate,
            servicePointIdentifier: servicePoint.Identifier,
            commodityId: servicePoint.CommodityId,
            customerClassId: 11,
            loadProfileId: 22,
            readCycleId: 33,
            isAmsMeter: true,
            meterIdentifier: "M1",
            addressLine1: "123 Main St",
            city: "Austin",
            state: "TX",
            zip: "78701",
            zip4: "1234",
            countyId: 44);

        await _handler.Handle(domainEvent, CancellationToken.None);

        serviceStart.ServiceDate.ShouldBe(serviceDate);
        serviceStart.ServiceOrderStatusId.ShouldBe(Constants.ServiceOrderStatuses.Scheduled);
        await _serviceStartRepository.Received(1).UpdateAsync(serviceStart, Arg.Any<CancellationToken>());

        servicePoint.CustomerClassId.ShouldBe(11);
        servicePoint.LoadProfileId.ShouldBe(22);
        servicePoint.ReadCycleId.ShouldBe(33);
        servicePoint.IsAmsMeter.ShouldBeTrue();
        servicePoint.Meters.ShouldHaveSingleItem().Identifier.ShouldBe("M1");

        premise.AddressLine1.ShouldBe("123 Main St");
        premise.City.ShouldBe("Austin");
        premise.State.ShouldBe("TX");
        premise.Zip.ShouldBe("78701");
        premise.Zip4.ShouldBe("1234");
        premise.CountyId.ShouldBe(44);

        await _servicePointRepository.Received(1).UpdateAsync(servicePoint, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DoesNotDuplicateMeterWhenAlreadyPresent()
    {
        var serviceStart = new ServiceStart { Id = 99 };
        var premise = new Premise();
        var servicePoint = new ServicePoint { Id = 7, Identifier = "SP1", CommodityId = 2, Premise = premise };
        servicePoint.Meters.Add(new Meter { Identifier = "M1" });

        _serviceStartRepository.GetByIdAsync(serviceStart.Id, Arg.Any<CancellationToken>())
            .Returns(serviceStart);

        _servicePointRepository
            .FirstOrDefaultAsync(Arg.Any<ServicePointByIdentifierAndCommoditySpec>(), Arg.Any<CancellationToken>())
            .Returns(servicePoint);

        var domainEvent = new ServiceStartScheduledEvent(
            serviceStartId: serviceStart.Id,
            serviceDate: new DateOnly(2025, 11, 12),
            servicePointIdentifier: servicePoint.Identifier,
            commodityId: servicePoint.CommodityId,
            customerClassId: 11,
            loadProfileId: 22,
            readCycleId: 33,
            isAmsMeter: false,
            meterIdentifier: "M1",
            addressLine1: "123 Main St",
            city: "Austin",
            state: "TX",
            zip: "78701",
            zip4: null,
            countyId: 44);

        await _handler.Handle(domainEvent, CancellationToken.None);

        servicePoint.Meters.ShouldHaveSingleItem();
    }
}
