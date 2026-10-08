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
              <SupplierDUNS>1890692841000</SupplierDUNS>
              <UtilityDUNS>957877905</UtilityDUNS>
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
                <UtilityName>CENTERPOINT ENERGY HOUSTON ELEC LLC</UtilityName>
                <UtilityAccountNumber>1008901015213617648100</UtilityAccountNumber>
              </Usage>
            </Transaction>
            """);

        var item = _reader.Read(transaction);

        item.Purpose.ShouldBe("05");
        item.SubPurpose.ShouldBe("DD");
        item.TrackingNumber.ShouldBe("8670320251111F000471021495");
        item.OriginalTrackingNumber.ShouldBe("8670320251027F000467009762");
        item.ServicePointIdentifier.ShouldBe("1008901015213617648100");
    }

    [Fact]
    public void TreatsMissingOptionalElementsAsEmptyOrNull()
    {
        var transaction = XElement.Parse("""
            <Transaction>
              <SupplierDUNS>1890692841000</SupplierDUNS>
              <UtilityDUNS>957877905</UtilityDUNS>
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

        item.Purpose.ShouldBe("SU");
        item.SubPurpose.ShouldBe(string.Empty);
        item.TrackingNumber.ShouldBe("8670420251112000471518183");
        item.OriginalTrackingNumber.ShouldBeNull();
        item.ServicePointIdentifier.ShouldBe("1008901023807534840100");
    }
}
