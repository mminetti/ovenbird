using System.Xml.Linq;
using Core.Market;

namespace Infrastructure.Services.Market.BigData.Readers;

public class InvoiceTransactionReader : IMarketDocumentTransactionReader
{
    public const string Set = "810";

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
