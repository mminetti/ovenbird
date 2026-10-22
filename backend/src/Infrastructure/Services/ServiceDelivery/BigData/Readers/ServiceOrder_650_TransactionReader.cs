using System.Xml.Linq;
using Core.MarketExchange;

namespace Infrastructure.Services.ServiceDelivery.BigData.Readers;

public class ServiceOrder_650_TransactionReader : IMarketDocumentTransactionReader
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
