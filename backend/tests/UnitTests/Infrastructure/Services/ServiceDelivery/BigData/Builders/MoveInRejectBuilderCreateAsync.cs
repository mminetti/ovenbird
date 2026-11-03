using Ardalis.Result;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Infrastructure.Services.ServiceDelivery.BigData.Builders;
using UseCases.Interfaces.ServiceDelivery;

namespace UnitTests.Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class MoveInRejectBuilderCreateAsync
{
    private readonly IRepository<ServiceStartItem> _serviceStartItemRepository = Substitute.For<IRepository<ServiceStartItem>>();
    private readonly IServiceStartService _serviceStartService = Substitute.For<IServiceStartService>();
    private readonly MoveInReject_814_17_ImportBuilder _builder;

    public MoveInRejectBuilderCreateAsync()
    {
        _builder = new MoveInReject_814_17_ImportBuilder(
            _serviceStartItemRepository,
            _serviceStartService);
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

    private const string XmlWithoutService = """
        <Transaction>
          <TransactionSet>814</TransactionSet>
          <TransactionSubSet>17</TransactionSubSet>
          <AccountMaint>
            <ProcessTrackingNumber>181362701952511072501494563</ProcessTrackingNumber>
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
