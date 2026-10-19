using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Infrastructure.Services.Market.BigData.Builders;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class HistoricalUsageEntityBuilderCreateAsync
{
    private readonly IBigDataValueMapper _valueMapper = Substitute.For<IBigDataValueMapper>();
    private readonly IRepository<HistoricalUsage> _historicalUsageRepository = Substitute.For<IRepository<HistoricalUsage>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository = Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly HistoricalUsage_867_02_ImportBuilder _builder;

    public HistoricalUsageEntityBuilderCreateAsync()
    {
        _builder = new HistoricalUsage_867_02_ImportBuilder(
            _valueMapper,
            _historicalUsageRepository,
            _referenceRepository);

        _valueMapper.GetCommodityId(Arg.Any<string>()).Returns(Constants.Commodities.Electricity);
        _valueMapper.GetUnitOfMeasureId(Arg.Any<string>()).Returns(Constants.UnitOfMeasures.KilowattHour);
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

    [Fact]
    public async Task ThrowsBecauseServiceResolutionIsNotYetImplemented()
    {
        // Service resolution for 867_02 transactions is an explicit TODO (HistoricalUsage_867_02_ImportBuilder.GetServiceAsync) -
        // this documents that CreateAsync currently cannot complete until that is implemented.
        await Should.ThrowAsync<NotImplementedException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));
    }

    [Fact]
    public async Task NeverCallsPersistenceBeforeServiceIsResolved()
    {
        await Should.ThrowAsync<NotImplementedException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));

        await _historicalUsageRepository.DidNotReceive().AddAsync(Arg.Any<HistoricalUsage>(), Arg.Any<CancellationToken>());
        await _referenceRepository.DidNotReceive().AddAsync(Arg.Any<MarketDocumentReference>(), Arg.Any<CancellationToken>());
    }
}
