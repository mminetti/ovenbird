using Ardalis.Result;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Core.Subscription;
using Infrastructure.Services.ServiceDelivery.BigData.Builders;

namespace UnitTests.Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class HistoricalUsageEntityBuilderCreateAsync
{
    private readonly IBigDataValueMapper _valueMapper = Substitute.For<IBigDataValueMapper>();
    private readonly IRepository<ServiceStartItem> _serviceStartItemRepository = Substitute.For<IRepository<ServiceStartItem>>();
    private readonly IRepository<HistoricalUsage> _historicalUsageRepository = Substitute.For<IRepository<HistoricalUsage>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository = Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly HistoricalUsage_867_02_ImportBuilder _builder;

    public HistoricalUsageEntityBuilderCreateAsync()
    {
        _builder = new HistoricalUsage_867_02_ImportBuilder(
            _valueMapper,
            _serviceStartItemRepository,
            _historicalUsageRepository,
            _referenceRepository);

        _valueMapper.GetCommodityId(Arg.Any<string>()).Returns(Result.Success(Constants.Commodities.Electricity));
        _valueMapper.GetUnitOfMeasureId(Arg.Any<string>()).Returns(Result.Success(Constants.UnitOfMeasures.KilowattHour));
    }

    private const string Xml = """
        <Transaction>
          <TransactionSet>867</TransactionSet>
          <TransactionSubSet>02</TransactionSubSet>
          <Commodity>E</Commodity>
          <Usage>
            <UtilityAccountNumber>ACCT-1</UtilityAccountNumber>
            <TranNr814>814-REF</TranNr814>
            <UsageMeterList>
              <UsageMeter>
                <MeterIdentifier>MTR-1</MeterIdentifier>
                <ServicePeriodBeginDate>20260601</ServicePeriodBeginDate>
                <ServicePeriodEndDate>20260630</ServicePeriodEndDate>
                <UsageQuantityList>
                  <UsageQuantity>
                    <UsageReadList>
                      <UsageRead>
                        <ReadConsumption>100</ReadConsumption>
                        <ReadUOM>KH</ReadUOM>
                      </UsageRead>
                    </UsageReadList>
                  </UsageQuantity>
                </UsageQuantityList>
              </UsageMeter>
              <UsageMeter>
                <MeterIdentifier>MTR-2</MeterIdentifier>
                <ServicePeriodBeginDate>20260601</ServicePeriodBeginDate>
                <ServicePeriodEndDate>20260630</ServicePeriodEndDate>
                <UsageQuantityList>
                  <UsageQuantity>
                    <UsageReadList>
                      <UsageRead>
                        <ReadConsumption>200</ReadConsumption>
                        <ReadUOM>KH</ReadUOM>
                      </UsageRead>
                    </UsageReadList>
                  </UsageQuantity>
                </UsageQuantityList>
              </UsageMeter>
            </UsageMeterList>
          </Usage>
        </Transaction>
        """;

    private MarketDocumentItem CreateItem() => new()
    {
        Id = 42,
        Set = "867",
        SubSet = "02",
        ReferenceNumber = "REF-1",
        Raw = Xml,
    };

    private ServiceStartItem CreateExistingServiceStartItem()
    {
        var servicePoint = new ServicePoint
        {
            Id = 20,
            Identifier = "SP-1",
            Meters =
            [
                new Meter { Id = 30, Identifier = "MTR-1" },
                new Meter { Id = 31, Identifier = "MTR-2" },
            ],
        };

        var service = new Service { Id = 50, ServicePointId = servicePoint.Id, ServicePoint = servicePoint };
        var serviceStart = new ServiceStart { Id = 99, ServiceId = service.Id, Service = service };

        return new ServiceStartItem
        {
            Id = 1,
            ServiceStartId = serviceStart.Id,
            Identifier = "814-REF",
            ServiceStart = serviceStart,
        };
    }

    [Fact]
    public async Task CreatesOneHistoricalUsagePerMeterReadAndLinksItToTheItem()
    {
        _serviceStartItemRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(CreateExistingServiceStartItem());

        long nextUsageId = 100;
        var createdUsages = new List<HistoricalUsage>();
        _historicalUsageRepository
            .AddAsync(Arg.Do<HistoricalUsage>(createdUsages.Add), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var usage = callInfo.Arg<HistoricalUsage>();
                usage.Id = nextUsageId++;
                return usage;
            });

        var createdReferences = new List<MarketDocumentReference>();
        _referenceRepository
            .AddAsync(Arg.Do<MarketDocumentReference>(createdReferences.Add), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<MarketDocumentReference>());

        var item = CreateItem();

        var result = await _builder.CreateAsync(item, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        createdUsages.Count.ShouldBe(2);

        createdUsages[0].ServiceId.ShouldBe(50);
        createdUsages[0].ServicePointId.ShouldBe(20);
        createdUsages[0].MeterId.ShouldBe(30);
        createdUsages[0].PeriodStartDate.ShouldBe(new DateOnly(2026, 6, 1));
        createdUsages[0].PeriodEndDate.ShouldBe(new DateOnly(2026, 6, 30));
        createdUsages[0].Consumption.ShouldBe(100);
        createdUsages[0].CommodityId.ShouldBe(Constants.Commodities.Electricity);
        createdUsages[0].UnitOfMeasureId.ShouldBe(Constants.UnitOfMeasures.KilowattHour);

        createdUsages[1].MeterId.ShouldBe(31);
        createdUsages[1].Consumption.ShouldBe(200);

        createdReferences.Count.ShouldBe(2);
        createdReferences.ShouldAllBe(r =>
            r.MarketDocumentItemId == item.Id && r.ReferenceTypeId == Constants.MarketDocumentReferenceTypes.HistoricalUsage);
        createdReferences[0].ReferenceId.ShouldBe(createdUsages[0].Id);
        createdReferences[1].ReferenceId.ShouldBe(createdUsages[1].Id);
    }

    [Fact]
    public async Task ReturnsInvalidWhenServiceCannotBeResolved()
    {
        _serviceStartItemRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((ServiceStartItem?)null);

        var result = await _builder.CreateAsync(CreateItem(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task NeverCallsPersistenceWhenServiceCannotBeResolved()
    {
        _serviceStartItemRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((ServiceStartItem?)null);

        await _builder.CreateAsync(CreateItem(), CancellationToken.None);

        await _historicalUsageRepository.DidNotReceive().AddAsync(Arg.Any<HistoricalUsage>(), Arg.Any<CancellationToken>());
        await _referenceRepository.DidNotReceive().AddAsync(Arg.Any<MarketDocumentReference>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LeavesMeterIdNullWhenMeterIdentifierCannotBeResolved()
    {
        var existingServiceStartItem = CreateExistingServiceStartItem();
        existingServiceStartItem.ServiceStart.Service.ServicePoint.Meters = [];

        _serviceStartItemRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(existingServiceStartItem);

        _historicalUsageRepository
            .AddAsync(Arg.Any<HistoricalUsage>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<HistoricalUsage>());

        _referenceRepository
            .AddAsync(Arg.Any<MarketDocumentReference>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<MarketDocumentReference>());

        var result = await _builder.CreateAsync(CreateItem(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _historicalUsageRepository.Received(2).AddAsync(
            Arg.Is<HistoricalUsage>(x => x.MeterId == null),
            Arg.Any<CancellationToken>());
    }
}
