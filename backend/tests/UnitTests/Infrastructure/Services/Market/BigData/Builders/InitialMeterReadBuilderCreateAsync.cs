using Core.MarketExchange;
using Core.ServiceDelivery;
using Infrastructure.Services.Market.BigData.Builders;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class InitialMeterReadBuilderCreateAsync
{
    private readonly IRepository<ServiceStartItem> _serviceStartItemRepository = Substitute.For<IRepository<ServiceStartItem>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository = Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly InitialMeterRead_867_04_Builder _builder;

    public InitialMeterReadBuilderCreateAsync()
    {
        _builder = new InitialMeterRead_867_04_Builder(
            _serviceStartItemRepository,
            _referenceRepository);
    }

    private const string Xml = """
        <Transaction>
          <SupplierDUNS>1234567890123</SupplierDUNS>
          <UtilityDUNS>123456789</UtilityDUNS>
          <TransactionSet>867</TransactionSet>
          <TransactionSubSet>04</TransactionSubSet>
          <State>TX</State>
          <Commodity>E</Commodity>
          <Usage>
            <PurposeCode>SU</PurposeCode>
            <TransactionReferenceNumber>8670420251112000471536499</TransactionReferenceNumber>
            <UtilityName>ACME UTILITY CO</UtilityName>
            <EGSName>ACME ENERGY SUPPLIER</EGSName>
            <CreateDateTime>20251112000000</CreateDateTime>
            <TimestampReceived>20251112132945</TimestampReceived>
            <UtilityAccountNumber>1008901010186127443100</UtilityAccountNumber>
            <TranNr814>814-REF</TranNr814>
            <UsageMeterList>
              <UsageMeter>
                <UsageType>INT</UsageType>
                <MeterIdentifier>I92452587</MeterIdentifier>
                <MeterUnmeterFlag>M</MeterUnmeterFlag>
                <SwitchDate>20251112</SwitchDate>
                <UsageQuantityList>
                  <UsageQuantity>
                    <Qualifier>QD</Qualifier>
                    <UsageReadList>
                      <UsageRead>
                        <ReadUOM>KH</ReadUOM>
                        <TOU>51</TOU>
                        <StartMeterRead>62178</StartMeterRead>
                      </UsageRead>
                    </UsageReadList>
                  </UsageQuantity>
                </UsageQuantityList>
              </UsageMeter>
            </UsageMeterList>
          </Usage>
        </Transaction>
        """;

    private static MarketDocumentItem CreateItem() => new()
    {
        Id = 42,
        Set = "867",
        SubSet = "04",
        ReferenceNumber = "8670420251112000471536499",
        Raw = Xml,
    };

    [Fact]
    public async Task ThrowsBecauseServiceStartResolutionIsNotYetImplemented()
    {
        // ServiceStart resolution for 867_04 transactions is an explicit TODO (InitialMeterRead_867_04_Builder.GetServiceStartAsync) -
        // this documents that CreateAsync currently cannot complete until that is implemented.
        await Should.ThrowAsync<NotImplementedException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));
    }

    [Fact]
    public async Task NeverCallsPersistenceBeforeServiceStartIsResolved()
    {
        await Should.ThrowAsync<NotImplementedException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));

        await _serviceStartItemRepository.DidNotReceive().AddAsync(Arg.Any<ServiceStartItem>(), Arg.Any<CancellationToken>());
        await _referenceRepository.DidNotReceive().AddAsync(Arg.Any<MarketDocumentReference>(), Arg.Any<CancellationToken>());
    }
}
