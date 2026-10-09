using System.Xml.Linq;
using Core.Market;

namespace UseCases.Market.MarketDocuments.Import.BigData.Readers;

public class InvoiceTransactionReader : IMarketDocumentTransactionReader
{
    public const string Identifier = "810_02";

    public MarketDocumentItem Read(XElement transactionElement)
    {
        var transaction = transactionElement.Element("Invoice");

        return new MarketDocumentItem
        {
            ReferenceNumber = transaction?.Element("BillNumber")?.Value ?? string.Empty,
            ServicePointIdentifier = transaction?.Element("LDCAccountNumber")?.Value ?? string.Empty,
        };
    }
}
