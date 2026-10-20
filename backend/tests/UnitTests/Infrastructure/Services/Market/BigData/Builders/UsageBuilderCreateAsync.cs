using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Core.Subscription;
using Infrastructure.Services.Market.BigData.Builders;

namespace UnitTests.Infrastructure.Services.Market.BigData.Builders;

public class UsageBuilderCreateAsync
{
    private readonly IReadRepository<Usage> _usageReadRepository = Substitute.For<IReadRepository<Usage>>();
    private readonly IBigDataValueMapper _valueMapper = Substitute.For<IBigDataValueMapper>();
    private readonly IRepository<Usage> _usageRepository = Substitute.For<IRepository<Usage>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository = Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly IRepository<ServiceStartItem> _serviceStartItemRepository = Substitute.For<IRepository<ServiceStartItem>>();
    private readonly IReadRepository<Service> _serviceReadRepository = Substitute.For<IReadRepository<Service>>();
    private readonly Usage_867_03_ImportBuilder _builder;

    public UsageBuilderCreateAsync()
    {
        _builder = new Usage_867_03_ImportBuilder(
            _usageReadRepository,
            _valueMapper,
            _usageRepository,
            _referenceRepository,
            _serviceStartItemRepository,
            _serviceReadRepository);

        _valueMapper.GetCommodityId(Arg.Any<string>()).Returns(Constants.Commodities.Electricity);
        _valueMapper.GetUnitOfMeasureId(Arg.Any<string>()).Returns(Constants.UnitOfMeasures.KilowattHour);
        _valueMapper.GetUsageTypeId(Arg.Any<string>()).Returns(Constants.UsagePurposes.Original);

        _usageRepository
            .AddAsync(Arg.Any<Usage>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Usage>());

        _referenceRepository
            .AddAsync(Arg.Any<MarketDocumentReference>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<MarketDocumentReference>());
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

    private const string FinalXml = """
        <Transaction>
          <TransactionSet>867</TransactionSet>
          <TransactionSubSet>03</TransactionSubSet>
          <Commodity>E</Commodity>
          <Usage>
            <PurposeCode>00</PurposeCode>
            <FinalIndicator>F</FinalIndicator>
            <TransactionReferenceNumber>8670300Z82062258318491Z004</TransactionReferenceNumber>
            <UtilityAccountNumber>10443720004239415</UtilityAccountNumber>
            <TranNr814>814-REF</TranNr814>
            <UsageMeterList>
              <UsageMeter>
                <UsageType>DET</UsageType>
                <ServicePeriodBeginDate>20260604</ServicePeriodBeginDate>
                <ServicePeriodEndDate>20260706</ServicePeriodEndDate>
                <MeterIdentifier>135300358LG</MeterIdentifier>
                <UsageQuantityList>
                  <UsageQuantity>
                    <UsageReadList>
                      <UsageRead>
                        <ReadConsumption>2920</ReadConsumption>
                        <ReadUOM>KH</ReadUOM>
                        <StartMeterRead>2727</StartMeterRead>
                        <EndMeterRead>5647</EndMeterRead>
                      </UsageRead>
                    </UsageReadList>
                  </UsageQuantity>
                </UsageQuantityList>
              </UsageMeter>
            </UsageMeterList>
          </Usage>
        </Transaction>
        """;

    private const string TwoMetersXml = """
        <Transaction>
          <TransactionSet>867</TransactionSet>
          <TransactionSubSet>03</TransactionSubSet>
          <Commodity>E</Commodity>
          <Usage>
            <PurposeCode>00</PurposeCode>
            <TransactionReferenceNumber>8670300Z82062258318491Z004</TransactionReferenceNumber>
            <UtilityAccountNumber>10443720004239415</UtilityAccountNumber>
            <UsageMeterList>
              <UsageMeter>
                <UsageType>DET</UsageType>
                <ServicePeriodBeginDate>20260101</ServicePeriodBeginDate>
                <ServicePeriodEndDate>20260131</ServicePeriodEndDate>
                <MeterIdentifier>MTR-A</MeterIdentifier>
                <UsageQuantityList>
                  <UsageQuantity>
                    <UsageReadList>
                      <UsageRead>
                        <ReadConsumption>100</ReadConsumption>
                        <ReadUOM>KH</ReadUOM>
                      </UsageRead>
                    </UsageReadList>
                  </UsageQuantity>
                </UsageQuantityList>
              </UsageMeter>
              <UsageMeter>
                <UsageType>DET</UsageType>
                <ServicePeriodBeginDate>20260201</ServicePeriodBeginDate>
                <ServicePeriodEndDate>20260228</ServicePeriodEndDate>
                <MeterIdentifier>MTR-B</MeterIdentifier>
                <UsageQuantityList>
                  <UsageQuantity>
                    <UsageReadList>
                      <UsageRead>
                        <ReadConsumption>200</ReadConsumption>
                        <ReadUOM>KH</ReadUOM>
                      </UsageRead>
                    </UsageReadList>
                  </UsageQuantity>
                </UsageQuantityList>
              </UsageMeter>
            </UsageMeterList>
          </Usage>
        </Transaction>
        """;

    private static MarketDocumentItem CreateItem(string xml) => new()
    {
        Id = 42,
        Set = "867",
        SubSet = "03",
        ReferenceNumber = "8670300Z82062258318491Z004",
        Raw = xml,
    };

    private static Service CreateService(long id, long servicePointId, string servicePointIdentifier, DateOnly? startDate, DateOnly? endDate, params Meter[] meters)
    {
        var servicePoint = new ServicePoint { Id = servicePointId, Identifier = servicePointIdentifier, Meters = meters };
        return new Service { Id = id, ServicePointId = servicePointId, ServicePoint = servicePoint, StartDate = startDate, EndDate = endDate };
    }

    private static ServiceStartItem CreateServiceStartItemFor(Service service, string identifier)
    {
        var serviceStart = new ServiceStart { Id = 99, ServiceId = service.Id, Service = service };
        return new ServiceStartItem { Id = 1, ServiceStartId = serviceStart.Id, Identifier = identifier, ServiceStart = serviceStart };
    }

    [Fact]
    public async Task ResolvesServiceViaTranNr814WhenFinal()
    {
        var service = CreateService(50, 20, "SP-1", null, null, new Meter { Id = 30, Identifier = "135300358LG" });

        _serviceStartItemRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns(CreateServiceStartItemFor(service, "814-REF"));

        var createdUsages = new List<Usage>();
        _usageRepository.AddAsync(Arg.Do<Usage>(createdUsages.Add), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Usage>());

        await _builder.CreateAsync(CreateItem(FinalXml), CancellationToken.None);

        createdUsages.Count.ShouldBe(1);
        createdUsages[0].ServiceId.ShouldBe(50);
        createdUsages[0].ServicePointId.ShouldBe(20);
        createdUsages[0].MeterId.ShouldBe(30);
        createdUsages[0].IsFinal.ShouldBeTrue();

        await _serviceReadRepository.DidNotReceive().ListAsync(Arg.Any<ServiceByServicePointAndPeriodSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResolvesServiceViaDateRangeFallbackWhenNotFinal()
    {
        var service = CreateService(60, 20, "10443720004239415", new DateOnly(2026, 1, 1), null, new Meter { Id = 30, Identifier = "135300358LG" });

        _serviceReadRepository
            .ListAsync(Arg.Any<ServiceByServicePointAndPeriodSpec>(), Arg.Any<CancellationToken>())
            .Returns([service]);

        var createdUsages = new List<Usage>();
        _usageRepository.AddAsync(Arg.Do<Usage>(createdUsages.Add), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Usage>());

        await _builder.CreateAsync(CreateItem(Xml), CancellationToken.None);

        createdUsages.Count.ShouldBe(1);
        createdUsages[0].ServiceId.ShouldBe(60);
        createdUsages[0].ServicePointId.ShouldBe(20);
        createdUsages[0].MeterId.ShouldBe(30);
        createdUsages[0].IsFinal.ShouldBeFalse();

        await _serviceStartItemRepository.DidNotReceive().FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FallsBackToDateRangeWhenFinalButTranNr814LookupMisses()
    {
        _serviceStartItemRepository
            .FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>())
            .Returns((ServiceStartItem?)null);

        var service = CreateService(70, 20, "10443720004239415", new DateOnly(2026, 1, 1), null, new Meter { Id = 30, Identifier = "135300358LG" });

        _serviceReadRepository
            .ListAsync(Arg.Any<ServiceByServicePointAndPeriodSpec>(), Arg.Any<CancellationToken>())
            .Returns([service]);

        var createdUsages = new List<Usage>();
        _usageRepository.AddAsync(Arg.Do<Usage>(createdUsages.Add), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Usage>());

        await _builder.CreateAsync(CreateItem(FinalXml), CancellationToken.None);

        createdUsages.Count.ShouldBe(1);
        createdUsages[0].ServiceId.ShouldBe(70);

        await _serviceStartItemRepository.Received(1).FirstOrDefaultAsync(Arg.Any<ServiceStartItemByIdentifierSpec>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ThrowsWhenNoServiceMatchesDateRange()
    {
        _serviceReadRepository
            .ListAsync(Arg.Any<ServiceByServicePointAndPeriodSpec>(), Arg.Any<CancellationToken>())
            .Returns([]);

        await Should.ThrowAsync<InvalidOperationException>(
            () => _builder.CreateAsync(CreateItem(Xml), CancellationToken.None));

        await _usageRepository.DidNotReceive().AddAsync(Arg.Any<Usage>(), Arg.Any<CancellationToken>());
        await _referenceRepository.DidNotReceive().AddAsync(Arg.Any<MarketDocumentReference>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ThrowsWhenMultipleServicesMatchDateRange()
    {
        var serviceA = CreateService(60, 20, "10443720004239415", new DateOnly(2026, 1, 1), null);
        var serviceB = CreateService(61, 21, "10443720004239415", new DateOnly(2026, 1, 1), null);

        _serviceReadRepository
            .ListAsync(Arg.Any<ServiceByServicePointAndPeriodSpec>(), Arg.Any<CancellationToken>())
            .Returns([serviceA, serviceB]);

        await Should.ThrowAsync<InvalidOperationException>(
            () => _builder.CreateAsync(CreateItem(Xml), CancellationToken.None));

        await _usageRepository.DidNotReceive().AddAsync(Arg.Any<Usage>(), Arg.Any<CancellationToken>());
        await _referenceRepository.DidNotReceive().AddAsync(Arg.Any<MarketDocumentReference>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResolvesDifferentServicePerMeterInSameTransaction()
    {
        var serviceA = CreateService(80, 20, "10443720004239415", new DateOnly(2026, 1, 1), null, new Meter { Id = 30, Identifier = "MTR-A" });
        var serviceB = CreateService(81, 21, "10443720004239415", new DateOnly(2026, 2, 1), null, new Meter { Id = 31, Identifier = "MTR-B" });

        _serviceReadRepository
            .ListAsync(Arg.Any<ServiceByServicePointAndPeriodSpec>(), Arg.Any<CancellationToken>())
            .Returns([serviceA], [serviceB]);

        var createdUsages = new List<Usage>();
        _usageRepository.AddAsync(Arg.Do<Usage>(createdUsages.Add), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Usage>());

        await _builder.CreateAsync(CreateItem(TwoMetersXml), CancellationToken.None);

        createdUsages.Count.ShouldBe(2);
        createdUsages[0].ServiceId.ShouldBe(80);
        createdUsages[0].ServicePointId.ShouldBe(20);
        createdUsages[0].MeterId.ShouldBe(30);
        createdUsages[1].ServiceId.ShouldBe(81);
        createdUsages[1].ServicePointId.ShouldBe(21);
        createdUsages[1].MeterId.ShouldBe(31);
    }
}
