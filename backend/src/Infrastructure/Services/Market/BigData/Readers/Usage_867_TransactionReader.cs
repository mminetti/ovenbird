using System.Xml.Linq;
using Core.MarketExchange;

namespace Infrastructure.Services.Market.BigData.Readers;

public class Usage_867_TransactionReader : IMarketDocumentTransactionReader
{
    public const string Set = "867";

    public MarketDocumentItem Read(XElement transactionElement)
    {
        var transaction = transactionElement.Element("Usage");

        return new MarketDocumentItem
        {
            ReferenceNumber = transaction?.Element("TransactionReferenceNumber")?.Value ?? string.Empty,
            ServicePointIdentifier = transaction?.Element("UtilityAccountNumber")?.Value ?? string.Empty,
        };
    }
}
