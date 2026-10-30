using Ardalis.Result;
using Core.MarketExchange;
using Infrastructure.Services.ServiceDelivery.BigData.Builders;

namespace UnitTests.Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class EnrollmentNotification_814_05_ServicePointUpdateBuilderBuildAsync
{
    private readonly EnrollmentNotification_814_05_ServicePointUpdateBuilder _builder = new();

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
                <UtilityAccountNumber>1008901017189885037100</UtilityAccountNumber>
                <ServiceDateList>
                  <ServiceDate>
                    <CustomerAgreementDate>20251112</CustomerAgreementDate>
                  </ServiceDate>
                </ServiceDateList>
                <ServiceMeterList>
                  <ServiceMeter>
                    <MeterNumber>I88362388</MeterNumber>
                    <LDCRateClass>RESIDENTIAL SERVICE</LDCRateClass>
                    <LoadProfileCode>RESHIWR_COAST_IDR_WS_NOTOU</LoadProfileCode>
                    <OwnerMeterCycle>14</OwnerMeterCycle>
                    <AdvanceMeterInfo>AMSR</AdvanceMeterInfo>
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
    public async Task ReturnsRequestParsedFromAccountMaint()
    {
        var result = await _builder.BuildAsync(CreateItem(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();

        var request = result.Value;
        request.ServicePointIdentifier.ShouldBe("1008901017189885037100");
        request.CommodityCode.ShouldBe("E");
        request.CustomerClassCode.ShouldBe("RESIDENTIAL SERVICE");
        request.LoadProfileCode.ShouldBe("RESHIWR_COAST_IDR_WS_NOTOU");
        request.ReadCycleCode.ShouldBe("14");
        request.AdvanceMeterInfoCode.ShouldBe("AMSR");
        request.MeterNumber.ShouldBe("I88362388");
        request.PremiseAddressLine1.ShouldBe("123 MAIN ST");
        request.PremiseCity.ShouldBe("AUSTIN");
        request.PremiseState.ShouldBe("TX");
        request.PremisePostalCode.ShouldBe("787010000");
        request.CountyName.ShouldBe("TRAVIS");
    }

    [Fact]
    public async Task ReturnsInvalidWhenServiceIsMissing()
    {
        const string xmlWithoutService = """
            <Transaction>
              <Commodity>E</Commodity>
              <AccountMaint>
                <TransactionNumber>1</TransactionNumber>
              </AccountMaint>
            </Transaction>
            """;

        var item = new MarketDocumentItem { Id = 1, Set = "814", SubSet = "05", Raw = xmlWithoutService };

        var result = await _builder.BuildAsync(item, CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
    }
}
