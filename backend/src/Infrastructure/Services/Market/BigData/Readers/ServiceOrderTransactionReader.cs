using System.Xml.Linq;
using Core.Market;

namespace Infrastructure.Services.Market.BigData.Readers;

public class ServiceOrderTransactionReader : IMarketDocumentTransactionReader
{
    public const string Set = "650";

    public MarketDocumentItem Read(XElement transactionElement)
    {
        var transaction = transactionElement.Element("ServiceOrder");

        return new MarketDocumentItem
        {
            ReferenceNumber = transaction?.Element("ReferenceNumber")?.Value ?? string.Empty,
            ServicePointIdentifier = transaction?.Element("ServiceList")?.Elements("Service")
                .FirstOrDefault()?.Element("LDCAccountNumber")?.Value ?? string.Empty,
        };
    }
}
