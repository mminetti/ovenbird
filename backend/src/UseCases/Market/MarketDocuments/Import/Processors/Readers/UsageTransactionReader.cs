using System.Xml.Linq;
using Core.Market;

namespace UseCases.Market.MarketDocuments.Import.Processors.Readers;

public class UsageTransactionReader : IMarketDocumentTransactionReader
{
    public string Set => "867";

    public MarketDocumentItem Read(XElement transactionElement)
    {
        var usage = transactionElement.Element("Usage");

        return new MarketDocumentItem
        {
            Purpose = usage?.Element("PurposeCode")?.Value ?? string.Empty,
            SubPurpose = usage?.Element("ReportType")?.Value ?? string.Empty,
            TrackingNumber = usage?.Element("TransactionReferenceNumber")?.Value ?? string.Empty,
            OriginalTrackingNumber = usage?.Element("OriginalTransactionNumber")?.Value,
            ServicePointIdentifier = usage?.Element("UtilityAccountNumber")?.Value ?? string.Empty,
        };
    }
}
