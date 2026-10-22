using Ardalis.Result;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Infrastructure.Services.ServiceDelivery.BigData.Builders;

namespace UnitTests.Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class InitialMeterReadBuilderCreateAsync
{
    private readonly IRepository<ServiceStartItem> _serviceStartItemRepository = Substitute.For<IRepository<ServiceStartItem>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository = Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly InitialMeterRead_867_04_ImportBuilder _builder;

    public InitialMeterReadBuilderCreateAsync()
    {
        _builder = new InitialMeterRead_867_04_ImportBuilder(
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
    public async Task ReturnsInvalidWhenServiceStartCannotBeResolved()
    {
        _serviceStartItemRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((ServiceStartItem?)null);

        var result = await _builder.CreateAsync(CreateItem(), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }

    [Fact]
    public async Task NeverCallsPersistenceWhenServiceStartCannotBeResolved()
    {
        _serviceStartItemRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((ServiceStartItem?)null);

        await _builder.CreateAsync(CreateItem(), CancellationToken.None);

        await _serviceStartItemRepository.DidNotReceive().AddAsync(Arg.Any<ServiceStartItem>(), Arg.Any<CancellationToken>());
        await _referenceRepository.DidNotReceive().AddAsync(Arg.Any<MarketDocumentReference>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CompletesServiceStartWhenResolved()
    {
        var serviceStart = new ServiceStart { Id = 99 };
        var existingServiceStartItem = new ServiceStartItem
        {
            Id = 1,
            ServiceStartId = serviceStart.Id,
            Identifier = "814-REF",
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

        var result = await _builder.CreateAsync(item, CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await _serviceStartItemRepository.Received(1).AddAsync(
            Arg.Is<ServiceStartItem>(x => x.ServiceStartId == serviceStart.Id && x.Identifier == item.ReferenceNumber),
            Arg.Any<CancellationToken>());

        serviceStart.ServiceDate.ShouldBe(new DateOnly(2025, 11, 12));
        serviceStart.ServiceOrderStatusId.ShouldBe(Constants.ServiceOrderStatuses.Completed);
    }
}
