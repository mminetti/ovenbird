using Core.Market;
using Core.Service;
using Core.Service.Specifications;
using Infrastructure.Services.Market.BigData.Builders;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class ConfirmationBuilderCreateAsync
{
    private readonly IReadRepository<ServiceStart> _serviceStartReadRepository = Substitute.For<IReadRepository<ServiceStart>>();
    private readonly IRepository<ServiceStartItem> _serviceStartItemRepository = Substitute.For<IRepository<ServiceStartItem>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository = Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly EnrollmentResponse_814_05_Builder _builder;

    public ConfirmationBuilderCreateAsync()
    {
        _builder = new EnrollmentResponse_814_05_Builder(
            _serviceStartReadRepository,
            _serviceStartItemRepository,
            _referenceRepository);
    }

    private const string Xml = """
        <Transaction>
          <SupplierDUNS>1234567890123</SupplierDUNS>
          <UtilityDUNS>123456789</UtilityDUNS>
          <TransactionSet>814</TransactionSet>
          <TransactionSubSet>05</TransactionSubSet>
          <State>TX</State>
          <Commodity>E</Commodity>
          <AccountMaint>
            <ResponseRequestFlag>S</ResponseRequestFlag>
            <TransactionNumber>164535844520251112112933189069</TransactionNumber>
            <ProcessTrackingNumber>181451702318211122510245294</ProcessTrackingNumber>
            <UtilityName>ACME UTILITY CO</UtilityName>
            <ESPName>ACME ENERGY SUPPLIER</ESPName>
            <DateStampCreated>20251112</DateStampCreated>
            <DateStampReceived>20251112</DateStampReceived>
            <TimeStampReceived>113334</TimeStampReceived>
            <ServiceList>
              <Service>
                <ProductID>EL</ProductID>
                <ServiceRequested>CE</ServiceRequested>
                <SecondServiceRequested>MVI</SecondServiceRequested>
                <LineTrackNumber>1</LineTrackNumber>
                <ActionCode>A</ActionCode>
                <UtilityAccountNumber>1008901017189885037100</UtilityAccountNumber>
                <StdPointLocationCode>BAR</StdPointLocationCode>
                <ThirdServiceRequested>HU</ThirdServiceRequested>
                <DistributionLoss>D</DistributionLoss>
                <PremiseType>01</PremiseType>
                <SpecialNeedsInfo>N</SpecialNeedsInfo>
                <ServiceDateList>
                  <ServiceDate>
                    <CustomerAgreementDate>20251112</CustomerAgreementDate>
                  </ServiceDate>
                </ServiceDateList>
                <ServiceMeterList>
                  <ServiceMeter>
                    <MeterNumber>I88362388</MeterNumber>
                    <MeterAction>Q</MeterAction>
                    <BillDataAndIntervalType>KHMON</BillDataAndIntervalType>
                    <LDCRateClass>RESIDENTIAL SERVICE</LDCRateClass>
                    <LoadProfileCode>RESHIWR_COAST_IDR_WS_NOTOU</LoadProfileCode>
                    <OwnerMeterCycle>14</OwnerMeterCycle>
                    <AdvanceMeterInfo>AMSR</AdvanceMeterInfo>
                    <MeterConfiguration>M01</MeterConfiguration>
                    <MeterConfigDesc>HOUSE</MeterConfigDesc>
                    <ServiceMeterTOUList>
                      <ServiceMeterTOU>
                        <MeterType>KHMON</MeterType>
                        <MeterMultiplier>1</MeterMultiplier>
                        <TimeOfUse1>51</TimeOfUse1>
                      </ServiceMeterTOU>
                      <ServiceMeterTOU>
                        <MeterType>KHMON</MeterType>
                        <NumberOfDials>5.0</NumberOfDials>
                        <TimeOfUse1>51</TimeOfUse1>
                      </ServiceMeterTOU>
                    </ServiceMeterTOUList>
                  </ServiceMeter>
                </ServiceMeterList>
              </Service>
            </ServiceList>
          </AccountMaint>
        </Transaction>
        """;

    private static MarketDocumentItem CreateItem() => new()
    {
        Id = 42,
        Set = "814",
        SubSet = "05",
        ReferenceNumber = "164535844520251112112933189069",
        Raw = Xml,
    };

    [Fact]
    public async Task ThrowsBecauseAccountResolutionIsNotYetImplemented()
    {
        // Account resolution for 814 transactions is an explicit TODO (ServiceRequestReaderBase.GetAccountAsync) -
        // this documents that CreateAsync currently cannot complete until that is implemented.
        await Should.ThrowAsync<NotImplementedException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));
    }

    [Fact]
    public async Task NeverCallsServiceStartOrPersistenceBeforeAccountIsResolved()
    {
        await Should.ThrowAsync<NotImplementedException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));

        await _serviceStartReadRepository.DidNotReceive().FirstOrDefaultAsync(Arg.Any<ServiceStartByAccountIdSpec>(), Arg.Any<CancellationToken>());
        await _serviceStartItemRepository.DidNotReceive().AddAsync(Arg.Any<ServiceStartItem>(), Arg.Any<CancellationToken>());
        await _referenceRepository.DidNotReceive().AddAsync(Arg.Any<MarketDocumentReference>(), Arg.Any<CancellationToken>());
    }
}
