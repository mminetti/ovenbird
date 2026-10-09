using System.Xml.Linq;
using Infrastructure.Services.Market.BigData.Readers;

namespace UnitTests.Infrastructure.Services.Market.BigData.Readers;

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
                <UtilityName>ACME UTILITY CO</UtilityName>
                <UtilityAccountNumber>1008901015213617648100</UtilityAccountNumber>
              </Usage>
            </Transaction>
            """);

        var item = _reader.Read(transaction);

        item.ReferenceNumber.ShouldBe("8670320251111F000471021495");
        item.ServicePointIdentifier.ShouldBe("1008901015213617648100");
    }
}
