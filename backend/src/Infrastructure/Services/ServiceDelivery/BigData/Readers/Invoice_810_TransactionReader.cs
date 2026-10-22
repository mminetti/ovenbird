using System.Xml.Linq;
using Core.MarketExchange;

namespace Infrastructure.Services.ServiceDelivery.BigData.Readers;

public class Invoice_810_TransactionReader : IMarketDocumentTransactionReader
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
