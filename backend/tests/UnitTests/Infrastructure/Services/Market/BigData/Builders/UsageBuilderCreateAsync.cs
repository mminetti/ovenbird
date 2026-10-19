using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Infrastructure.Services.Market.BigData.Builders;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class UsageBuilderCreateAsync
{
    private readonly IReadRepository<Usage> _usageReadRepository = Substitute.For<IReadRepository<Usage>>();
    private readonly IBigDataValueMapper _valueMapper = Substitute.For<IBigDataValueMapper>();
    private readonly IRepository<Usage> _usageRepository = Substitute.For<IRepository<Usage>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository = Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly Usage_867_03_ImportBuilder _builder;

    public UsageBuilderCreateAsync()
    {
        _builder = new Usage_867_03_ImportBuilder(
            _usageReadRepository,
            _valueMapper,
            _usageRepository,
            _referenceRepository);

        _valueMapper.GetCommodityId(Arg.Any<string>()).Returns(Constants.Commodities.Electricity);
        _valueMapper.GetUnitOfMeasureId(Arg.Any<string>()).Returns(Constants.UnitOfMeasures.KilowattHour);
        _valueMapper.GetUsageTypeId(Arg.Any<string>()).Returns(Constants.UsagePurposes.Original);
    }

    private const string Xml = """
        <Transaction>
          <SupplierDUNS>1234567890123</SupplierDUNS>
          <UtilityDUNS>123456789</UtilityDUNS>
          <TransactionSet>867</TransactionSet>
          <TransactionSubSet>03</TransactionSubSet>
          <State>TX</State>
          <Commodity>E</Commodity>
          <Usage>
            <PurposeCode>00</PurposeCode>
            <ReportType>DD</ReportType>
            <TransactionReferenceNumber>8670300Z82062258318491Z004</TransactionReferenceNumber>
            <UtilityName>ACME UTILITY CO</UtilityName>
            <EGSName>ACME ENERGY SUPPLIER</EGSName>
            <CreateDateTime>20260707000000</CreateDateTime>
            <TimestampReceived>20260706213742</TimestampReceived>
            <UtilityAccountNumber>10443720004239415</UtilityAccountNumber>
            <TranNr814>814-REF</TranNr814>
            <UsageMeterList>
              <UsageMeter>
                <UsageType>SUM</UsageType>
                <ServicePeriodBeginDate>20260604</ServicePeriodBeginDate>
                <ServicePeriodEndDate>20260706</ServicePeriodEndDate>
                <MeterUOM>KH</MeterUOM>
                <IntervalType>MON</IntervalType>
                <UsageQuantityList>
                  <UsageQuantity>
                    <Qualifier>QD</Qualifier>
                    <Quantity>2920</Quantity>
                    <UsageReadList>
                      <UsageRead>
                        <ReadConsumption>2920</ReadConsumption>
                        <ReadUOM>KH</ReadUOM>
                        <TOU>51</TOU>
                      </UsageRead>
                    </UsageReadList>
                  </UsageQuantity>
                </UsageQuantityList>
              </UsageMeter>
              <UsageMeter>
                <UsageType>DET</UsageType>
                <ServicePeriodBeginDate>20260604</ServicePeriodBeginDate>
                <ServicePeriodEndDate>20260706</ServicePeriodEndDate>
                <MeterIdentifier>135300358LG</MeterIdentifier>
                <MeterUOM>KH</MeterUOM>
                <IntervalType>MON</IntervalType>
                <MeterRole>A</MeterRole>
                <MeterUnmeterFlag>M</MeterUnmeterFlag>
                <UsageQuantityList>
                  <UsageQuantity>
                    <Qualifier>QD</Qualifier>
                    <Quantity>2920</Quantity>
                    <UsageReadList>
                      <UsageRead>
                        <ReadConsumption>2920</ReadConsumption>
                        <ReadUOM>KH</ReadUOM>
                        <ReadFlag>AA</ReadFlag>
                        <TOU>51</TOU>
                        <StartMeterRead>2727</StartMeterRead>
                        <EndMeterRead>5647</EndMeterRead>
                        <Multiplier>1</Multiplier>
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
        SubSet = "03",
        ReferenceNumber = "8670300Z82062258318491Z004",
        Raw = Xml,
    };

    [Fact]
    public async Task ThrowsBecauseServiceResolutionIsNotYetImplemented()
    {
        // Service resolution for 867_03 transactions is an explicit TODO (Usage_867_03_ImportBuilder.GetServiceAsync) -
        // this documents that CreateAsync currently cannot complete until that is implemented.
        await Should.ThrowAsync<NotImplementedException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));
    }

    [Fact]
    public async Task NeverCallsPersistenceBeforeServiceIsResolved()
    {
        await Should.ThrowAsync<NotImplementedException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));

        await _usageRepository.DidNotReceive().AddAsync(Arg.Any<Usage>(), Arg.Any<CancellationToken>());
        await _referenceRepository.DidNotReceive().AddAsync(Arg.Any<MarketDocumentReference>(), Arg.Any<CancellationToken>());
    }
}
