using System.Xml.Linq;
using Core.Market;

namespace Infrastructure.Services.Market.BigData.Readers;

public class UsageTransactionReader : IMarketDocumentTransactionReader
{
    public const string Identifier = "867_03";

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
