using System.Xml.Linq;
using UseCases.Market.MarketDocuments.Import.BigData.Readers;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class InvoiceTransactionReaderRead
{
    private readonly InvoiceTransactionReader _reader = new();

    [Fact]
    public void MapsAllFieldsWhenPresent()
    {
        var transaction = XElement.Parse("""
            <Transaction>
              <SupplierDUNS>1890692841000</SupplierDUNS>
              <UtilityDUNS>007923311</UtilityDUNS>
              <TransactionSet>810</TransactionSet>
              <TransactionSubSet>02</TransactionSubSet>
              <State>TX</State>
              <Commodity>E</Commodity>
              <Invoice>
                <BillDate>20260702</BillDate>
                <BillNumber>97X1260702220427505973</BillNumber>
                <CrossRefNumber>970631306720260702205530734744</CrossRefNumber>
                <BillActionCode>PR</BillActionCode>
                <BillPurpose>00</BillPurpose>
                <LDCAccountNumber>10204049751086976</LDCAccountNumber>
                <LDCName>AEP TEXAS NORTH (ERCOT)</LDCName>
              </Invoice>
            </Transaction>
            """);

        var item = _reader.Read(transaction);

        item.Purpose.ShouldBe("00");
        item.SubPurpose.ShouldBe("PR");
        item.TrackingNumber.ShouldBe("97X1260702220427505973");
        item.OriginalTrackingNumber.ShouldBe("970631306720260702205530734744");
        item.ServicePointIdentifier.ShouldBe("10204049751086976");
    }

    [Fact]
    public void TreatsMissingCrossRefNumberAsNull()
    {
        var transaction = XElement.Parse("""
            <Transaction>
              <TransactionSet>810</TransactionSet>
              <TransactionSubSet>02</TransactionSubSet>
              <Invoice>
                <BillNumber>97X1260702220427505973</BillNumber>
                <BillActionCode>PR</BillActionCode>
                <BillPurpose>00</BillPurpose>
                <LDCAccountNumber>10204049751086976</LDCAccountNumber>
              </Invoice>
            </Transaction>
            """);

        var item = _reader.Read(transaction);

        item.OriginalTrackingNumber.ShouldBeNull();
    }
}
