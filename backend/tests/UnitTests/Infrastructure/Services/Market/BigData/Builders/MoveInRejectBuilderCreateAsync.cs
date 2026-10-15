using Core.MarketExchange;
using Core.ServiceDelivery;
using Infrastructure.Services.Market.BigData.Builders;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class MoveInRejectBuilderCreateAsync
{
    private readonly IRepository<ServiceStartItem> _serviceStartItemRepository = Substitute.For<IRepository<ServiceStartItem>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository = Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly MoveInReject_814_17_Builder _builder;

    public MoveInRejectBuilderCreateAsync()
    {
        _builder = new MoveInReject_814_17_Builder(
            _serviceStartItemRepository,
            _referenceRepository);
    }

    private const string Xml = """
        <Transaction>
          <SupplierDUNS>1234567890123</SupplierDUNS>
          <UtilityDUNS>123456789</UtilityDUNS>
          <TransactionSet>814</TransactionSet>
          <TransactionSubSet>17</TransactionSubSet>
          <State>TX</State>
          <Commodity>E</Commodity>
          <AccountMaint>
            <ResponseRequestFlag>S</ResponseRequestFlag>
            <TransactionNumber>163778614020251107135933189069</TransactionNumber>
            <ProcessTrackingNumber>181362701952511072501494563</ProcessTrackingNumber>
            <UtilityName>ACME UTILITY CO</UtilityName>
            <ESPName>ACME ENERGY SUPPLIER</ESPName>
            <DateStampCreated>20251107</DateStampCreated>
            <DateStampReceived>20251107</DateStampReceived>
            <TimeStampReceived>140601</TimeStampReceived>
            <ServiceList>
              <Service>
                <ProductID>EL</ProductID>
                <ServiceRequested>CE</ServiceRequested>
                <SecondServiceRequested>MVI</SecondServiceRequested>
                <LineTrackNumber>1</LineTrackNumber>
                <ActionCode>R</ActionCode>
                <UtilityAccountNumber>1008901017189885037100</UtilityAccountNumber>
                <ThirdServiceRequested>HU</ThirdServiceRequested>
                <ServiceRejectReasonList>
                  <ServiceRejectReason>
                    <RejectCode>FRG</RejectCode>
                    <RejectReason>Invalid Bill Calculator (REF~PC)</RejectReason>
                  </ServiceRejectReason>
                </ServiceRejectReasonList>
              </Service>
            </ServiceList>
          </AccountMaint>
        </Transaction>
        """;

    private static MarketDocumentItem CreateItem() => new()
    {
        Id = 42,
        Set = "814",
        SubSet = "17",
        ReferenceNumber = "163778614020251107135933189069",
        Raw = Xml,
    };

    [Fact]
    public async Task ThrowsBecauseServiceStartResolutionIsNotYetImplemented()
    {
        // ServiceStart resolution for 814 transactions is an explicit TODO (MoveInReject_814_17_Builder.GetServiceStartAsync) -
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
