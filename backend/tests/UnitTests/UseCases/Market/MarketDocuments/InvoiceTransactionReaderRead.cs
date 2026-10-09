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
              <SupplierDUNS>1234567890123</SupplierDUNS>
              <UtilityDUNS>123456789</UtilityDUNS>
              <TransactionSet>810</TransactionSet>
              <TransactionSubSet>02</TransactionSubSet>
              <State>TX</State>
              <Commodity>E</Commodity>
              <Invoice>
                <BillDate>20260702</BillDate>
                <BillNumber>97X1260702220427505973</BillNumber>
                <BillActionCode>PR</BillActionCode>
                <BillPurpose>00</BillPurpose>
                <LDCAccountNumber>10204049751086976</LDCAccountNumber>
                <LDCName>ACME UTILITY CO</LDCName>
              </Invoice>
            </Transaction>
            """);

        var item = _reader.Read(transaction);

        item.ReferenceNumber.ShouldBe("97X1260702220427505973");
        item.ServicePointIdentifier.ShouldBe("10204049751086976");
    }
}
