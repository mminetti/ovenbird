using Ardalis.Result;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;
using Infrastructure.Services.ServiceDelivery.BigData.Builders;
using UseCases.Interfaces.ServiceDelivery;
using Core.Common.Constants;

namespace UnitTests.Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class ConfirmationBuilderCreateAsync
{
    private readonly IRepository<ServiceStartItem> _serviceStartItemRepository = Substitute.For<IRepository<ServiceStartItem>>();
    private readonly IRepository<ServiceStart> _serviceStartRepository = Substitute.For<IRepository<ServiceStart>>();
    private readonly IRepository<ServiceEvent> _serviceEventRepository = Substitute.For<IRepository<ServiceEvent>>();
    private readonly IServiceStartService _serviceStartService = Substitute.For<IServiceStartService>();
    private readonly IUpdateServicePointService _updateServicePointService = Substitute.For<IUpdateServicePointService>();
    private readonly EnrollmentNotification_814_05_ImportBuilder _builder;

    public ConfirmationBuilderCreateAsync()
    {
        _updateServicePointService
            .ExecuteAsync(Arg.Any<UpdateServicePointRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        _builder = new EnrollmentNotification_814_05_ImportBuilder(
            _serviceStartItemRepository,
            _serviceStartRepository,
            _serviceEventRepository,
            _serviceStartService,
            _updateServicePointService);
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
            <NameAddressList>
              <NameAddress>
                <EntityType>8R</EntityType>
                <EntityName>JOHN DOE</EntityName>
                <EntityAddress1>123 MAIN ST</EntityAddress1>
                <EntityCity>AUSTIN</EntityCity>
                <EntityState>TX</EntityState>
                <EntityPostalCode>787010000</EntityPostalCode>
                <EntityCounty>TRAVIS</EntityCounty>
              </NameAddress>
            </NameAddressList>
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

    private const string XmlWithoutServiceStartDate = """
        <Transaction>
          <TransactionSet>814</TransactionSet>
          <TransactionSubSet>05</TransactionSubSet>
          <AccountMaint>
            <ProcessTrackingNumber>181451702318211122510245294</ProcessTrackingNumber>
            <ServiceList>
              <Service>
                <ProductID>EL</ProductID>
                <UtilityAccountNumber>1008901017189885037100</UtilityAccountNumber>
              </Service>
            </ServiceList>
          </AccountMaint>
        </Transaction>
        """;

    private void SetUpServiceStartNotFound() =>
        _serviceStartService
            .GetByIdentifierAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(Result.Invalid(new ValidationError(nameof(ServiceStart), "ServiceStart with identifier was not found.")));

    [Fact]
    public async Task ReturnsInvalidWhenRawIsNotValidXml()
    {
        var item = CreateItem();
        item.Raw = "<Transaction><AccountMaint>";

        var result = await _builder.CreateAsync(item, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);

        await _serviceStartItemRepository.DidNotReceive().AddAsync(Arg.Any<ServiceStartItem>(), Arg.Any<CancellationToken>());
    }

    private static MarketDocumentItem CreateItem() => new()
    {
        Id = 42,
        Set = "814",
        SubSet = "05",
        ReferenceNumber = "164535844520251112112933189069",
        Raw = Xml,
    };

    [Fact]
    public async Task ReturnsBothErrorsWhenServiceStartCannotBeResolvedAndServiceStartDateIsMissing()
    {
        SetUpServiceStartNotFound();

        var item = CreateItem();
        item.Raw = XmlWithoutServiceStartDate;

        var result = await _builder.CreateAsync(item, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.Count().ShouldBe(2);
        result.ValidationErrors.ShouldContain(e => e.Identifier == nameof(ServiceStart));
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage == "Service Start Date is required.");
    }

    [Fact]
    public async Task ReturnsInvalidWhenServiceStartCannotBeResolved()
    {
        SetUpServiceStartNotFound();

        var result = await _builder.CreateAsync(CreateItem(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task NeverCallsPersistenceWhenServiceStartCannotBeResolved()
    {
        SetUpServiceStartNotFound();

        await _builder.CreateAsync(CreateItem(), CancellationToken.None);

        await _serviceStartItemRepository.DidNotReceive().AddAsync(Arg.Any<ServiceStartItem>(), Arg.Any<CancellationToken>());
        await _serviceEventRepository.DidNotReceive().AddAsync(Arg.Any<ServiceEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ConfirmsServiceStartAndRaisesScheduledEventWhenResolved()
    {
        var serviceStart = new ServiceStart { Id = 99 };

        _serviceStartService
            .GetByIdentifierAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(serviceStart));

        _serviceStartItemRepository
            .AddAsync(Arg.Any<ServiceStartItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var item = callInfo.Arg<ServiceStartItem>();
                item.Id = 2;
                return item;
            });

        ServiceEvent? createdServiceEvent = null;

        _serviceEventRepository
            .AddAsync(Arg.Any<ServiceEvent>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var serviceEvent = callInfo.Arg<ServiceEvent>();
                serviceEvent.Id = 77;
                createdServiceEvent = serviceEvent;
                return serviceEvent;
            });

        var item = CreateItem();

        var result = await _builder.CreateAsync(item, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe([2L]);

        await _serviceStartItemRepository.Received(1).AddAsync(
            Arg.Is<ServiceStartItem>(x => x.ServiceStartId == serviceStart.Id && x.Identifier == item.ReferenceNumber),
            Arg.Any<CancellationToken>());

        await _serviceEventRepository.Received(1).AddAsync(
            Arg.Is<ServiceEvent>(x =>
                x.EventTypeId == Constants.ServiceEventTypes.ServiceStartConfirmed &&
                x.ReferenceTypeId == Constants.ServiceEventReferenceTypes.ServiceStartItem &&
                x.ReferenceId == 2),
            Arg.Any<CancellationToken>());

        serviceStart.ServiceDate.ShouldBe(new DateOnly(2025, 11, 12));
        serviceStart.ServiceOrderStatusId.ShouldBe(Constants.ServiceOrderStatuses.Scheduled);

        await _serviceStartRepository.Received(1).UpdateAsync(serviceStart, Arg.Any<CancellationToken>());

        await _serviceEventRepository.Received(1).UpdateAsync(
            Arg.Is<ServiceEvent>(x => x.Id == 77), Arg.Any<CancellationToken>());

        serviceStart.DomainEvents.ShouldBeEmpty();

        var domainEvent = createdServiceEvent.ShouldNotBeNull().DomainEvents.OfType<ServiceEventCreatedEvent>().ShouldHaveSingleItem();
        domainEvent.ServiceEventId.ShouldBe(77L);

        await _updateServicePointService.Received(1).ExecuteAsync(
            Arg.Is<UpdateServicePointRequest>(x =>
                x.ServicePointIdentifier == "1008901017189885037100" &&
                x.CommodityCode == "E" &&
                x.CustomerClassCode == "RESIDENTIAL SERVICE" &&
                x.LoadProfileCode == "RESHIWR_COAST_IDR_WS_NOTOU" &&
                x.ReadCycleCode == "14" &&
                x.AmsMeterCode == "AMSR" &&
                x.MeterNumber == "I88362388" &&
                x.PremiseAddressLine1 == "123 MAIN ST" &&
                x.PremiseCity == "AUSTIN" &&
                x.PremiseState == "TX" &&
                x.PremiseZipCode == "787010000" &&
                x.CountyName == "TRAVIS"),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ReturnsInvalidAndNeverPersistsWhenServicePointUpdateFails()
    {
        var serviceStart = new ServiceStart { Id = 99 };

        _serviceStartService
            .GetByIdentifierAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(serviceStart));

        _updateServicePointService
            .ExecuteAsync(Arg.Any<UpdateServicePointRequest>(), Arg.Any<CancellationToken>())
            .Returns(Result.Invalid(new ValidationError("ServicePoint", "ServicePoint '1008901017189885037100' was not found.")));

        var result = await _builder.CreateAsync(CreateItem(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);

        await _serviceStartItemRepository.DidNotReceive().AddAsync(Arg.Any<ServiceStartItem>(), Arg.Any<CancellationToken>());
        await _serviceEventRepository.DidNotReceive().AddAsync(Arg.Any<ServiceEvent>(), Arg.Any<CancellationToken>());
        await _serviceStartRepository.DidNotReceive().UpdateAsync(Arg.Any<ServiceStart>(), Arg.Any<CancellationToken>());
    }
}
