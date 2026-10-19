using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Infrastructure.Services.Market.BigData.Builders;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class UsageBuilderCreateAsync
{
    private readonly IReadRepository<Usage> _usageReadRepository = Substitute.For<IReadRepository<Usage>>();
    private readonly IReadRepository<ServicePoint> _servicePointReadRepository = Substitute.For<IReadRepository<ServicePoint>>();
    private readonly IBigDataValueMapper _valueMapper = Substitute.For<IBigDataValueMapper>();
    private readonly IRepository<Usage> _usageRepository = Substitute.For<IRepository<Usage>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository = Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly Usage_867_03_ImportBuilder _builder;

    public UsageBuilderCreateAsync()
    {
        _builder = new Usage_867_03_ImportBuilder(
            _usageReadRepository,
            _servicePointReadRepository,
            _valueMapper,
            _usageRepository,
            _referenceRepository);

        _valueMapper.GetCommodityId(Arg.Any<string>()).Returns(Constants.Commodities.Electricity);
        _valueMapper.GetUnitOfMeasureId(Arg.Any<string>()).Returns(Constants.UnitOfMeasures.KilowattHour);
        _valueMapper.GetUsageTypeId(Arg.Any<string>()).Returns(Constants.UsagePurposes.Original);

        var servicePoint = new ServicePoint
        {
            Id = 20,
            Identifier = "SP-1",
            Meters = [new Meter { Id = 30, Identifier = "135300358LG" }],
        };

        _servicePointReadRepository
            .FirstOrDefaultAsync(Arg.Any<ServicePointWithMetersByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(servicePoint);
    }

    private const string Xml = """
        <Transaction>
          <SupplierDUNS>1234567890123</SupplierDUNS>
          <UtilityDUNS>123456789</UtilityDUNS>
          <TransactionSet>867</TransactionSet>
          <TransactionSubSet>03</TransactionSubSet>
          <State>TX</State>
          <Commodity>E</Commodity>
          <Usage>
            <PurposeCode>00</PurposeCode>
            <ReportType>DD</ReportType>
            <TransactionReferenceNumber>8670300Z82062258318491Z004</TransactionReferenceNumber>
            <UtilityName>ACME UTILITY CO</UtilityName>
            <EGSName>ACME ENERGY SUPPLIER</EGSName>
            <CreateDateTime>20260707000000</CreateDateTime>
            <TimestampReceived>20260706213742</TimestampReceived>
            <UtilityAccountNumber>10443720004239415</UtilityAccountNumber>
            <TranNr814>814-REF</TranNr814>
            <UsageMeterList>
              <UsageMeter>
                <UsageType>SUM</UsageType>
                <ServicePeriodBeginDate>20260604</ServicePeriodBeginDate>
                <ServicePeriodEndDate>20260706</ServicePeriodEndDate>
                <MeterUOM>KH</MeterUOM>
                <IntervalType>MON</IntervalType>
                <UsageQuantityList>
                  <UsageQuantity>
                    <Qualifier>QD</Qualifier>
                    <Quantity>2920</Quantity>
                    <UsageReadList>
                      <UsageRead>
                        <ReadConsumption>2920</ReadConsumption>
                        <ReadUOM>KH</ReadUOM>
                        <TOU>51</TOU>
                      </UsageRead>
                    </UsageReadList>
                  </UsageQuantity>
                </UsageQuantityList>
              </UsageMeter>
              <UsageMeter>
                <UsageType>DET</UsageType>
                <ServicePeriodBeginDate>20260604</ServicePeriodBeginDate>
                <ServicePeriodEndDate>20260706</ServicePeriodEndDate>
                <MeterIdentifier>135300358LG</MeterIdentifier>
                <MeterUOM>KH</MeterUOM>
                <IntervalType>MON</IntervalType>
                <MeterRole>A</MeterRole>
                <MeterUnmeterFlag>M</MeterUnmeterFlag>
                <UsageQuantityList>
                  <UsageQuantity>
                    <Qualifier>QD</Qualifier>
                    <Quantity>2920</Quantity>
                    <UsageReadList>
                      <UsageRead>
                        <ReadConsumption>2920</ReadConsumption>
                        <ReadUOM>KH</ReadUOM>
                        <ReadFlag>AA</ReadFlag>
                        <TOU>51</TOU>
                        <StartMeterRead>2727</StartMeterRead>
                        <EndMeterRead>5647</EndMeterRead>
                        <Multiplier>1</Multiplier>
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
        SubSet = "03",
        ReferenceNumber = "8670300Z82062258318491Z004",
        Raw = Xml,
    };

    [Fact]
    public async Task CreatesOneUsagePerDetailMeterReadAndLinksItToTheItem()
    {
        long nextUsageId = 100;
        var createdUsages = new List<Usage>();
        _usageRepository
            .AddAsync(Arg.Do<Usage>(createdUsages.Add), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var usage = callInfo.Arg<Usage>();
                usage.Id = nextUsageId++;
                return usage;
            });

        var createdReferences = new List<MarketDocumentReference>();
        _referenceRepository
            .AddAsync(Arg.Do<MarketDocumentReference>(createdReferences.Add), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<MarketDocumentReference>());

        var item = CreateItem();

        await _builder.CreateAsync(item, CancellationToken.None);

        createdUsages.Count.ShouldBe(1);

        var usage = createdUsages[0];
        usage.Identifier.ShouldBe(item.ReferenceNumber);
        usage.ServicePointId.ShouldBe(20);
        usage.MeterId.ShouldBe(30);
        usage.UsageTypeId.ShouldBe(Constants.UsagePurposes.Original);
        usage.OriginalUsageId.ShouldBeNull();
        usage.IsFinal.ShouldBeFalse();
        usage.PeriodStartDate.ShouldBe(new DateOnly(2026, 6, 4));
        usage.PeriodEndDate.ShouldBe(new DateOnly(2026, 7, 6));
        usage.Consumption.ShouldBe(2920);
        usage.StartRead.ShouldBe(2727);
        usage.EndRead.ShouldBe(5647);
        usage.CommodityId.ShouldBe(Constants.Commodities.Electricity);
        usage.UnitOfMeasureId.ShouldBe(Constants.UnitOfMeasures.KilowattHour);
        usage.UsageStatusId.ShouldBe(Constants.UsageStatuses.New);

        createdReferences.Count.ShouldBe(1);
        createdReferences[0].MarketDocumentItemId.ShouldBe(item.Id);
        createdReferences[0].ReferenceTypeId.ShouldBe(Constants.MarketDocumentReferenceTypes.Usage);
        createdReferences[0].ReferenceId.ShouldBe(usage.Id);
    }

    [Fact]
    public async Task ResolvesOriginalUsageWhenOriginalTransactionNumberIsPresent()
    {
        var item = CreateItem();
        item.Raw = Xml.Replace(
            "<PurposeCode>00</PurposeCode>",
            "<PurposeCode>05</PurposeCode>\n    <OriginalTransactionNumber>ORIG-1</OriginalTransactionNumber>");

        _valueMapper.GetUsageTypeId("05").Returns(Constants.UsagePurposes.Replace);

        _usageReadRepository
            .FirstOrDefaultAsync(Arg.Any<UsageByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(new Usage { Id = 999, Identifier = "ORIG-1" });

        _usageRepository
            .AddAsync(Arg.Any<Usage>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Usage>());

        _referenceRepository
            .AddAsync(Arg.Any<MarketDocumentReference>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<MarketDocumentReference>());

        await _builder.CreateAsync(item, CancellationToken.None);

        await _usageRepository.Received(1).AddAsync(
            Arg.Is<Usage>(u => u.OriginalUsageId == 999 && u.UsageTypeId == Constants.UsagePurposes.Replace),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ThrowsWhenOriginalUsageCannotBeResolved()
    {
        var item = CreateItem();
        item.Raw = Xml.Replace(
            "<PurposeCode>00</PurposeCode>",
            "<PurposeCode>05</PurposeCode>\n    <OriginalTransactionNumber>ORIG-1</OriginalTransactionNumber>");

        _usageReadRepository
            .FirstOrDefaultAsync(Arg.Any<UsageByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((Usage?)null);

        await Should.ThrowAsync<InvalidOperationException>(
            () => _builder.CreateAsync(item, CancellationToken.None));
    }

    [Fact]
    public async Task ThrowsWhenMeterIdentifierCannotBeResolved()
    {
        _servicePointReadRepository
            .FirstOrDefaultAsync(Arg.Any<ServicePointWithMetersByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(new ServicePoint { Id = 20, Identifier = "SP-1", Meters = [] });

        await Should.ThrowAsync<InvalidOperationException>(
            () => _builder.CreateAsync(CreateItem(), CancellationToken.None));
    }
}
