using Ardalis.Result;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Infrastructure.Services.ServiceDelivery.BigData.Builders;
using UseCases.Interfaces.ServiceDelivery;

namespace UnitTests.Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class SwitchRejectBuilderCreateAsync
{
    private readonly IRepository<ServiceStartItem> _serviceStartItemRepository = Substitute.For<IRepository<ServiceStartItem>>();
    private readonly IServiceStartService _serviceStartService = Substitute.For<IServiceStartService>();
    private readonly SwitchReject_814_02_ImportBuilder _builder;

    public SwitchRejectBuilderCreateAsync()
    {
        _builder = new SwitchReject_814_02_ImportBuilder(
            _serviceStartItemRepository,
            _serviceStartService);
    }

    private const string Xml = """
        <Transaction>
          <SupplierDUNS>1234567890123</SupplierDUNS>
          <UtilityDUNS>123456789</UtilityDUNS>
          <TransactionSet>814</TransactionSet>
          <TransactionSubSet>02</TransactionSubSet>
          <State>TX</State>
          <Commodity>E</Commodity>
          <AccountMaint>
            <ResponseRequestFlag>S</ResponseRequestFlag>
            <TransactionNumber>164459368920251111185332189069</TransactionNumber>
            <ProcessTrackingNumber>181442709922511112506412439</ProcessTrackingNumber>
            <UtilityName>ACME UTILITY CO</UtilityName>
            <ESPName>ACME ENERGY SUPPLIER</ESPName>
            <DateStampCreated>20251111</DateStampCreated>
            <DateStampReceived>20251111</DateStampReceived>
            <TimeStampReceived>185722</TimeStampReceived>
            <ServiceList>
              <Service>
                <ProductID>EL</ProductID>
                <ServiceRequested>CE</ServiceRequested>
                <SecondServiceRequested>HU</SecondServiceRequested>
                <LineTrackNumber>1</LineTrackNumber>
                <ActionCode>R</ActionCode>
                <UtilityAccountNumber>1008901017189885037100</UtilityAccountNumber>
                <ThirdServiceRequested>SW</ThirdServiceRequested>
                <ServiceRejectReasonList>
                  <ServiceRejectReason>
                    <RejectCode>NFI</RejectCode>
                    <RejectReason>Not First In</RejectReason>
                  </ServiceRejectReason>
                </ServiceRejectReasonList>
              </Service>
            </ServiceList>
          </AccountMaint>
        </Transaction>
        """;

    private const string XmlWithoutService = """
        <Transaction>
          <TransactionSet>814</TransactionSet>
          <TransactionSubSet>02</TransactionSubSet>
          <AccountMaint>
            <ProcessTrackingNumber>181442709922511112506412439</ProcessTrackingNumber>
          </AccountMaint>
        </Transaction>
        """;

    private static MarketDocumentItem CreateItem() => new()
    {
        Id = 42,
        Set = "814",
        SubSet = "02",
        ReferenceNumber = "164459368920251111185332189069",
        Raw = Xml,
    };

    private void SetUpServiceStartNotFound() =>
        _serviceStartService
            .GetByIdentifierAsync(Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(Result.Invalid(new ValidationError(nameof(ServiceStart), "ServiceStart with identifier was not found.")));

    [Fact]
    public async Task ReturnsBothErrorsWhenServiceStartCannotBeResolvedAndThereIsNoService()
    {
        SetUpServiceStartNotFound();

        var item = CreateItem();
        item.Raw = XmlWithoutService;

        var result = await _builder.CreateAsync(item, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.Count().ShouldBe(2);
        result.ValidationErrors.ShouldContain(e => e.Identifier == nameof(ServiceStart));
        result.ValidationErrors.ShouldContain(e => e.ErrorMessage == $"MarketDocumentItem '{item.Id}' has no Service.");
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
    }

    [Fact]
    public async Task RejectsServiceStartWhenResolved()
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

        var item = CreateItem();

        var result = await _builder.CreateAsync(item, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe([2L]);
        await _serviceStartItemRepository.Received(1).AddAsync(
            Arg.Is<ServiceStartItem>(x => x.ServiceStartId == serviceStart.Id && x.Identifier == item.ReferenceNumber),
            Arg.Any<CancellationToken>());

        serviceStart.ServiceOrderStatusId.ShouldBe(Constants.ServiceOrderStatuses.Rejected);
    }
}
