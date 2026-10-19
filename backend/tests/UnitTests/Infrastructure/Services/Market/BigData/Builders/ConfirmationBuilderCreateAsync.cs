using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Infrastructure.Services.Market.BigData.Builders;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class ConfirmationBuilderCreateAsync
{
    private readonly IRepository<ServiceStartItem> _serviceStartItemRepository = Substitute.For<IRepository<ServiceStartItem>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository = Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly EnrollmentNotification_814_05_ImportBuilder _builder;

    public ConfirmationBuilderCreateAsync()
    {
        _builder = new EnrollmentNotification_814_05_ImportBuilder(
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
    public async Task ThrowsWhenServiceStartCannotBeResolved()
    {
        _serviceStartItemRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((ServiceStartItem?)null);

        await Should.ThrowAsync<InvalidOperationException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));
    }

    [Fact]
    public async Task NeverCallsPersistenceWhenServiceStartCannotBeResolved()
    {
        _serviceStartItemRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((ServiceStartItem?)null);

        await Should.ThrowAsync<InvalidOperationException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));

        await _serviceStartItemRepository.DidNotReceive().AddAsync(Arg.Any<ServiceStartItem>(), Arg.Any<CancellationToken>());
        await _referenceRepository.DidNotReceive().AddAsync(Arg.Any<MarketDocumentReference>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmsServiceStartWhenResolved()
    {
        var serviceStart = new ServiceStart { Id = 99 };
        var existingServiceStartItem = new ServiceStartItem
        {
            Id = 1,
            ServiceStartId = serviceStart.Id,
            Identifier = "181451702318211122510245294",
            ServiceStart = serviceStart,
        };

        _serviceStartItemRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(existingServiceStartItem);

        _serviceStartItemRepository
            .AddAsync(Arg.Any<ServiceStartItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var item = callInfo.Arg<ServiceStartItem>();
                item.Id = 2;
                return item;
            });

        _referenceRepository
            .AddAsync(Arg.Any<MarketDocumentReference>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<MarketDocumentReference>());

        var item = CreateItem();

        await _builder.CreateAsync(item, CancellationToken.None);

        await _serviceStartItemRepository.Received(1).AddAsync(
            Arg.Is<ServiceStartItem>(x => x.ServiceStartId == serviceStart.Id && x.Identifier == item.ReferenceNumber),
            Arg.Any<CancellationToken>());

        serviceStart.ServiceDate.ShouldBe(new DateOnly(2025, 11, 12));
        serviceStart.ServiceOrderStatusId.ShouldBe(Constants.ServiceOrderStatuses.Scheduled);
    }
}
