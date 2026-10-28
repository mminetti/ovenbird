using Ardalis.Result;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;
using Core.ServiceDelivery.Specifications;
using Infrastructure.Services.ServiceDelivery;
using Infrastructure.Services.ServiceDelivery.BigData.Builders;
using UseCases.Interfaces.ServiceDelivery;

namespace UnitTests.Infrastructure.Services.ServiceDelivery;

public class UpdateServicePointActionExecuteAsync
{
    private readonly IRepository<ServicePoint> _servicePointRepository = Substitute.For<IRepository<ServicePoint>>();
    private readonly IServiceDeliveryLookupService _lookupService = Substitute.For<IServiceDeliveryLookupService>();
    private readonly IBigDataValueMapper _valueMapper = Substitute.For<IBigDataValueMapper>();
    private readonly UpdateServicePointAction _action;

    public UpdateServicePointActionExecuteAsync()
    {
        _action = new UpdateServicePointAction(_servicePointRepository, _lookupService, _valueMapper);
    }

    private static ServiceStartScheduledEvent CreateDomainEvent(string? premiseAddressLine1 = "123 MAIN ST") => new(
        serviceEventId: 7,
        servicePointIdentifier: "1008901017189885037100",
        commodityCode: "E",
        customerClassCode: "RESIDENTIAL SERVICE",
        loadProfileCode: "RESHIWR_COAST_IDR_WS_NOTOU",
        readCycleCode: "14",
        advanceMeterInfoCode: "AMSR",
        meterNumber: "I88362388",
        premiseAddressLine1: premiseAddressLine1,
        premiseCity: "AUSTIN",
        premiseState: "TX",
        premisePostalCode: "787010000",
        countyName: "TRAVIS");

    private void SetUpSuccessfulLookups()
    {
        _valueMapper.GetCommodityId("E").Returns(Result.Success(2));
        _valueMapper.IsAmsMeter("AMSR").Returns(true);

        _lookupService.GetCountyByNameAsync("TRAVIS", Arg.Any<CancellationToken>())
            .Returns(Result.Success(new County { Id = 44, Name = "TRAVIS" }));
        _lookupService.GetCustomerClassByNameAsync("RESIDENTIAL SERVICE", Arg.Any<CancellationToken>())
            .Returns(Result.Success(new CustomerClass { Id = 11, Name = "RESIDENTIAL SERVICE" }));
        _lookupService.GetLoadProfileByNameAsync("RESHIWR_COAST_IDR_WS_NOTOU", Arg.Any<CancellationToken>())
            .Returns(Result.Success(new LoadProfile { Id = 22, Name = "RESHIWR_COAST_IDR_WS_NOTOU" }));
        _lookupService.GetReadCycleByNameAsync("14", Arg.Any<CancellationToken>())
            .Returns(Result.Success(new ReadCycle { Id = 33, Name = "14" }));
    }

    [Fact]
    public async Task UpdatesServicePointAndPremiseWhenAllLookupsSucceed()
    {
        SetUpSuccessfulLookups();

        var premise = new Premise();
        var servicePoint = new ServicePoint { Id = 7, Identifier = "1008901017189885037100", CommodityId = 2, Premise = premise };

        _servicePointRepository
            .FirstOrDefaultAsync(Arg.Any<ServicePointByIdentifierAndCommoditySpec>(), Arg.Any<CancellationToken>())
            .Returns(servicePoint);

        var result = await _action.ExecuteAsync(CreateDomainEvent(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();

        servicePoint.CustomerClassId.ShouldBe(11);
        servicePoint.LoadProfileId.ShouldBe(22);
        servicePoint.ReadCycleId.ShouldBe(33);
        servicePoint.IsAmsMeter.ShouldBeTrue();
        servicePoint.Meters.ShouldHaveSingleItem().Identifier.ShouldBe("I88362388");

        premise.AddressLine1.ShouldBe("123 MAIN ST");
        premise.City.ShouldBe("AUSTIN");
        premise.State.ShouldBe("TX");
        premise.Zip.ShouldBe("78701");
        premise.Zip4.ShouldBe("0000");
        premise.CountyId.ShouldBe(44);

        await _servicePointRepository.Received(1).UpdateAsync(servicePoint, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsInvalidWhenServicePointNotFound()
    {
        SetUpSuccessfulLookups();

        _servicePointRepository
            .FirstOrDefaultAsync(Arg.Any<ServicePointByIdentifierAndCommoditySpec>(), Arg.Any<CancellationToken>())
            .Returns((ServicePoint?)null);

        var result = await _action.ExecuteAsync(CreateDomainEvent(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _servicePointRepository.DidNotReceive().UpdateAsync(Arg.Any<ServicePoint>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsInvalidWhenPremiseAddressMissing()
    {
        _valueMapper.GetCommodityId("E").Returns(Result.Success(2));

        var result = await _action.ExecuteAsync(CreateDomainEvent(premiseAddressLine1: null), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _servicePointRepository.DidNotReceive().UpdateAsync(Arg.Any<ServicePoint>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsInvalidWhenCommodityCodeIsUnmapped()
    {
        _valueMapper.GetCommodityId("E").Returns(Result.Invalid(new ValidationError("Commodity", "Unmapped commodity 'E'.")));

        var result = await _action.ExecuteAsync(CreateDomainEvent(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        await _servicePointRepository.DidNotReceive().UpdateAsync(Arg.Any<ServicePoint>(), Arg.Any<CancellationToken>());
    }
}
