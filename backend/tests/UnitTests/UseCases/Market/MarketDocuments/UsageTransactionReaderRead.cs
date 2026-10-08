using System.Xml.Linq;
using UseCases.Market.MarketDocuments.Import.BigData.Readers;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class UsageTransactionReaderRead
{
    private readonly UsageTransactionReader _reader = new();

    [Fact]
    public void MapsAllFieldsWhenPresent()
    {
        var transaction = XElement.Parse("""
            <Transaction>
              <SupplierDUNS>1234567890123</SupplierDUNS>
              <UtilityDUNS>123456789</UtilityDUNS>
              <TransactionSet>867</TransactionSet>
              <TransactionSubSet>03</TransactionSubSet>
              <State>TX</State>
              <Commodity>E</Commodity>
              <Usage>
                <PurposeCode>05</PurposeCode>
                <ReportType>DD</ReportType>
                <FinalIndicator>F</FinalIndicator>
                <TransactionReferenceNumber>8670320251111F000471021495</TransactionReferenceNumber>
                <OriginalTransactionNumber>8670320251027F000467009762</OriginalTransactionNumber>
                <UtilityName>ACME UTILITY CO</UtilityName>
                <UtilityAccountNumber>1008901015213617648100</UtilityAccountNumber>
              </Usage>
            </Transaction>
            """);

        var item = _reader.Read(transaction);

        item.Purpose.ShouldBe("REPLACE");
        item.SubPurpose.ShouldBe("NONINTERVAL");
        item.TrackingNumber.ShouldBe("8670320251111F000471021495");
        item.OriginalTrackingNumber.ShouldBe("8670320251027F000467009762");
        item.ServicePointIdentifier.ShouldBe("1008901015213617648100");
    }

    [Fact]
    public void TreatsMissingOptionalElementsAsEmptyOrNull()
    {
        var transaction = XElement.Parse("""
            <Transaction>
              <SupplierDUNS>1234567890123</SupplierDUNS>
              <UtilityDUNS>123456789</UtilityDUNS>
              <TransactionSet>867</TransactionSet>
              <TransactionSubSet>04</TransactionSubSet>
              <State>TX</State>
              <Commodity>E</Commodity>
              <Usage>
                <PurposeCode>SU</PurposeCode>
                <TransactionReferenceNumber>8670420251112000471518183</TransactionReferenceNumber>
                <UtilityAccountNumber>1008901023807534840100</UtilityAccountNumber>
              </Usage>
            </Transaction>
            """);

        var item = _reader.Read(transaction);

        item.Purpose.ShouldBe("ORIGINAL");
        item.SubPurpose.ShouldBe("NONINTERVAL");
        item.TrackingNumber.ShouldBe("8670420251112000471518183");
        item.OriginalTrackingNumber.ShouldBeNull();
        item.ServicePointIdentifier.ShouldBe("1008901023807534840100");
    }

    [Fact]
    public void FlattensUsageIntoFields()
    {
        var transaction = XElement.Parse("""
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
                <TransactionReferenceNumber>945308221620260707214016448020</TransactionReferenceNumber>
                <UtilityName>ACME UTILITY CO</UtilityName>
                <EGSName>ACME POWER RETAILER</EGSName>
                <CreateDateTime>20260708000000</CreateDateTime>
                <TimestampReceived>20260708010036</TimestampReceived>
                <UtilityAccountNumber>10032789461659571</UtilityAccountNumber>
                <UsageMeterList>
                  <UsageMeter>
                    <UsageType>DET</UsageType>
                    <ServicePeriodBeginDate>20260605</ServicePeriodBeginDate>
                    <ServicePeriodEndDate>20260707</ServicePeriodEndDate>
                    <MeterIdentifier>200238219</MeterIdentifier>
                    <MeterUOM>KH</MeterUOM>
                    <IntervalType>MON</IntervalType>
                    <MeterRole>A</MeterRole>
                    <MeterUnmeterFlag>M</MeterUnmeterFlag>
                    <UsageQuantityList>
                      <UsageQuantity>
                        <Qualifier>QD</Qualifier>
                        <Quantity>666</Quantity>
                        <UsageReadList>
                          <UsageRead>
                            <ReadConsumption>666</ReadConsumption>
                            <ReadUOM>KH</ReadUOM>
                            <ReadFlag>AA</ReadFlag>
                            <TOU>51</TOU>
                            <StartMeterRead>9812</StartMeterRead>
                            <EndMeterRead>10478</EndMeterRead>
                            <Multiplier>1</Multiplier>
                          </UsageRead>
                        </UsageReadList>
                      </UsageQuantity>
                    </UsageQuantityList>
                  </UsageMeter>
                  <UsageMeter>
                    <UsageType>SUM</UsageType>
                    <ServicePeriodBeginDate>20260605</ServicePeriodBeginDate>
                    <ServicePeriodEndDate>20260707</ServicePeriodEndDate>
                    <MeterUOM>KH</MeterUOM>
                    <IntervalType>MON</IntervalType>
                    <UsageQuantityList>
                      <UsageQuantity>
                        <Qualifier>QD</Qualifier>
                        <Quantity>666</Quantity>
                        <UsageReadList>
                          <UsageRead>
                            <ReadConsumption>666</ReadConsumption>
                            <ReadUOM>KH</ReadUOM>
                            <TOU>51</TOU>
                            <Multiplier>1</Multiplier>
                          </UsageRead>
                        </UsageReadList>
                      </UsageQuantity>
                    </UsageQuantityList>
                  </UsageMeter>
                </UsageMeterList>
              </Usage>
            </Transaction>
            """);

        var item = _reader.Read(transaction);

        item.Fields.Count.ShouldBe(36);

        item.Fields.ShouldContain(f => f.FieldName == "Usage.PurposeCode" && f.FieldValue == "00");
        item.Fields.ShouldContain(f => f.FieldName == "Usage.UtilityAccountNumber" && f.FieldValue == "10032789461659571");

        item.Fields.ShouldContain(f => f.FieldName == "Usage.UsageMeter[0].UsageType" && f.FieldValue == "DET");
        item.Fields.ShouldContain(f => f.FieldName == "Usage.UsageMeter[0].MeterIdentifier" && f.FieldValue == "200238219");
        item.Fields.ShouldContain(f =>
            f.FieldName == "Usage.UsageMeter[0].UsageQuantity[0].UsageRead[0].ReadConsumption" && f.FieldValue == "666");
        item.Fields.ShouldContain(f =>
            f.FieldName == "Usage.UsageMeter[0].UsageQuantity[0].UsageRead[0].StartMeterRead" && f.FieldValue == "9812");

        item.Fields.ShouldContain(f => f.FieldName == "Usage.UsageMeter[1].UsageType" && f.FieldValue == "SUM");
        item.Fields.ShouldNotContain(f => f.FieldName == "Usage.UsageMeter[1].MeterIdentifier");
        item.Fields.ShouldContain(f =>
            f.FieldName == "Usage.UsageMeter[1].UsageQuantity[0].UsageRead[0].ReadConsumption" && f.FieldValue == "666");

        item.Fields.ShouldAllBe(f => !f.FieldName.Contains("List"));
    }
}
