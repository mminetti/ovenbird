using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Infrastructure.Services.Market.BigData.Builders;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class SwitchRejectBuilderCreateAsync
{
    private readonly IRepository<ServiceStartItem> _serviceStartItemRepository = Substitute.For<IRepository<ServiceStartItem>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository = Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly SwitchReject_814_02_ImportBuilder _builder;

    public SwitchRejectBuilderCreateAsync()
    {
        _builder = new SwitchReject_814_02_ImportBuilder(
            _serviceStartItemRepository,
            _referenceRepository);
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

    private static MarketDocumentItem CreateItem() => new()
    {
        Id = 42,
        Set = "814",
        SubSet = "02",
        ReferenceNumber = "164459368920251111185332189069",
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
    public async Task RejectsServiceStartWhenResolved()
    {
        var serviceStart = new ServiceStart { Id = 99 };
        var existingServiceStartItem = new ServiceStartItem
        {
            Id = 1,
            ServiceStartId = serviceStart.Id,
            Identifier = "181442709922511112506412439",
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

        serviceStart.ServiceOrderStatusId.ShouldBe(Constants.ServiceOrderStatuses.Rejected);
    }
}
