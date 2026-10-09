using Core.Common.Constants;
using Core.Market;
using Core.Service;
using Core.Subscription;
using Core.Subscription.Specifications;
using Infrastructure.Services.Market.BigData.EntityBuilders;

namespace UnitTests.Infrastructure.Services.Market.BigData.EntityBuilders;

public class HistoricalUsageEntityBuilderCreateAsync
{
    private readonly IReadRepository<Account> _accountReadRepository = Substitute.For<IReadRepository<Account>>();
    private readonly IBigDataLookupService _lookupService = Substitute.For<IBigDataLookupService>();
    private readonly IRepository<HistoricalUsage> _historicalUsageRepository = Substitute.For<IRepository<HistoricalUsage>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository = Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly HistoricalUsageEntityBuilder _builder;

    public HistoricalUsageEntityBuilderCreateAsync()
    {
        _builder = new HistoricalUsageEntityBuilder(
            _accountReadRepository,
            _lookupService,
            _historicalUsageRepository,
            _referenceRepository);

        _lookupService.GetCommodityId(Arg.Any<string>()).Returns(Constants.Commodities.Electricity);
        _lookupService.GetUnitOfMeasureId(Arg.Any<string>()).Returns(Constants.UnitOfMeasures.KilowattHour);
        _lookupService.GetServicePointId("ACCT-1", Arg.Any<CancellationToken>()).Returns(20L);
        _lookupService.GetMeterId("MTR-1", Arg.Any<CancellationToken>()).Returns((long?)30);
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

    [Fact]
    public async Task CreatesOneHistoricalUsagePerMeterReadAndLinksItToTheItem()
    {
        _accountReadRepository
            .FirstOrDefaultAsync(Arg.Any<AccountByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Account { Id = 10, Identifier = "ACCT-1" });

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

        await _builder.CreateAsync(item, CancellationToken.None);

        createdUsages.Count.ShouldBe(2);

        createdUsages[0].AccountId.ShouldBe(10);
        createdUsages[0].ServicePointId.ShouldBe(20);
        createdUsages[0].MeterId.ShouldBe(30);
        createdUsages[0].PeriodStartDate.ShouldBe(new DateOnly(2026, 6, 1));
        createdUsages[0].PeriodEndDate.ShouldBe(new DateOnly(2026, 6, 30));
        createdUsages[0].Consumption.ShouldBe(100);
        createdUsages[0].CommodityId.ShouldBe(Constants.Commodities.Electricity);
        createdUsages[0].UnitOfMeasureId.ShouldBe(Constants.UnitOfMeasures.KilowattHour);

        createdUsages[1].MeterId.ShouldBeNull();
        createdUsages[1].Consumption.ShouldBe(200);

        createdReferences.Count.ShouldBe(2);
        createdReferences.ShouldAllBe(r =>
            r.MarketDocumentItemId == item.Id && r.ReferenceTypeId == Constants.MarketDocumentReferenceTypes.HistoricalUsage);
        createdReferences[0].ReferenceId.ShouldBe(createdUsages[0].Id);
        createdReferences[1].ReferenceId.ShouldBe(createdUsages[1].Id);
    }

    [Fact]
    public async Task ThrowsWhenAccountCannotBeResolved()
    {
        _accountReadRepository
            .FirstOrDefaultAsync(Arg.Any<AccountByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((Account?)null);

        await Should.ThrowAsync<InvalidOperationException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));
    }

    [Fact]
    public async Task ThrowsWhenServicePointCannotBeResolved()
    {
        _accountReadRepository
            .FirstOrDefaultAsync(Arg.Any<AccountByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Account { Id = 10, Identifier = "ACCT-1" });

        _lookupService
            .GetServicePointId("ACCT-1", Arg.Any<CancellationToken>())
            .Returns(Task.FromException<long>(new InvalidOperationException("ServicePoint 'ACCT-1' was not found.")));

        await Should.ThrowAsync<InvalidOperationException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));
    }

    [Fact]
    public async Task ThrowsWhenMeterIdentifierIsPresentButCannotBeResolved()
    {
        _accountReadRepository
            .FirstOrDefaultAsync(Arg.Any<AccountByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Account { Id = 10, Identifier = "ACCT-1" });

        _lookupService
            .GetMeterId("MTR-1", Arg.Any<CancellationToken>())
            .Returns(Task.FromException<long?>(new InvalidOperationException("Meter 'MTR-1' was not found.")));

        await Should.ThrowAsync<InvalidOperationException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));
    }
}
