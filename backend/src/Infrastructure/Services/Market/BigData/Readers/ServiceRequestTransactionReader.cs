using System.Xml.Linq;
using Core.Market;

namespace Infrastructure.Services.Market.BigData.Readers;

public class ServiceRequestTransactionReader : IMarketDocumentTransactionReader
{
    public const string Set = "814";

    public MarketDocumentItem Read(XElement transactionElement)
    {
        var transaction = transactionElement.Element("AccountMaint");

        return new MarketDocumentItem
        {
            ReferenceNumber = transaction?.Element("TransactionNumber")?.Value ?? string.Empty,
            ServicePointIdentifier = transaction?.Element("ServiceList")?.Elements("Service")
                .FirstOrDefault()?.Element("UtilityAccountNumber")?.Value ?? string.Empty,
        };
    }
}
