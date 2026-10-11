using Core.Common.Constants;
using Core.Market;
using Core.Service;
using Core.Service.Specifications;
using Core.Subscription;
using Core.Subscription.Specifications;
using Infrastructure.Services.Market.BigData.Builders;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class InitialMeterReadBuilderCreateAsync
{
    private readonly IReadRepository<Account> _accountReadRepository = Substitute.For<IReadRepository<Account>>();
    private readonly IReadRepository<ServiceStart> _serviceStartReadRepository = Substitute.For<IReadRepository<ServiceStart>>();
    private readonly IRepository<ServiceStartItem> _serviceStartItemRepository = Substitute.For<IRepository<ServiceStartItem>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository = Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly InitialMeterRead_867_04_Builder _builder;

    public InitialMeterReadBuilderCreateAsync()
    {
        _builder = new InitialMeterRead_867_04_Builder(
            _accountReadRepository,
            _serviceStartReadRepository,
            _serviceStartItemRepository,
            _referenceRepository);

        _accountReadRepository
            .FirstOrDefaultAsync(Arg.Any<AccountByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Account { Id = 10, Identifier = "814-REF" });

        _serviceStartReadRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartByAccountIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(new ServiceStart { Id = 50, AccountId = 10 });
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

    private MarketDocumentItem CreateItem() => new()
    {
        Id = 42,
        Set = "867",
        SubSet = "04",
        ReferenceNumber = "8670420251112000471536499",
        Raw = Xml,
    };

    [Fact]
    public async Task CreatesOneServiceStartItemPerInitialMeterReadAndLinksItToTheItem()
    {
        long nextItemId = 100;
        var createdItems = new List<ServiceStartItem>();
        _serviceStartItemRepository
            .AddAsync(Arg.Do<ServiceStartItem>(createdItems.Add), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var serviceStartItem = callInfo.Arg<ServiceStartItem>();
                serviceStartItem.Id = nextItemId++;
                return serviceStartItem;
            });

        var createdReferences = new List<MarketDocumentReference>();
        _referenceRepository
            .AddAsync(Arg.Do<MarketDocumentReference>(createdReferences.Add), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<MarketDocumentReference>());

        var item = CreateItem();

        await _builder.CreateAsync(item, CancellationToken.None);

        createdItems.Count.ShouldBe(1);

        var serviceStartItem = createdItems[0];
        serviceStartItem.ServiceStartId.ShouldBe(50);
        serviceStartItem.Identifier.ShouldBe(item.ReferenceNumber);
        serviceStartItem.ServiceDate.ShouldBe(new DateOnly(2025, 11, 12));
        serviceStartItem.ServiceActionId.ShouldBe(Constants.ServiceActions.Complete);

        createdReferences.Count.ShouldBe(1);
        createdReferences[0].MarketDocumentItemId.ShouldBe(item.Id);
        createdReferences[0].ReferenceTypeId.ShouldBe(Constants.MarketDocumentReferenceTypes.ServiceStart);
        createdReferences[0].ReferenceId.ShouldBe(serviceStartItem.Id);
    }

    [Fact]
    public async Task ThrowsWhenAccountCannotBeResolved()
    {
        _accountReadRepository
            .FirstOrDefaultAsync(Arg.Any<AccountByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((Account?)null);

        await Should.ThrowAsync<InvalidOperationException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));
    }

    [Fact]
    public async Task ThrowsWhenServiceStartCannotBeResolved()
    {
        _serviceStartReadRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartByAccountIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((ServiceStart?)null);

        await Should.ThrowAsync<InvalidOperationException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));
    }
}
