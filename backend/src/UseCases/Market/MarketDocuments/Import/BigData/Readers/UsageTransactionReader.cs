using System.Xml.Linq;
using Core.Market;

namespace UseCases.Market.MarketDocuments.Import.BigData.Readers;

public class UsageTransactionReader : IMarketDocumentTransactionReader
{
    public const string Identifier = "867_03";

    public MarketDocumentItem Read(XElement transactionElement)
    {
        var transaction = transactionElement.Element("Usage");

        var purposeCode = transaction?.Element("PurposeCode")?.Value ?? string.Empty;
        var purpose = purposeCode switch
        {
            "01" => "CANCEL",
            "05" => "REPLACE",
            "07" => "DUPLICATE",
            _ => "ORIGINAL",
        };

        var reportType = transaction?.Element("ReportType")?.Value ?? string.Empty;
        var subPurpose = reportType switch
        {
            "C1" => "INTERVAL",
            _ => "NONINTERVAL",
        };

        return new MarketDocumentItem
        {
            Purpose = purpose,
            SubPurpose = subPurpose,
            TrackingNumber = transaction?.Element("TransactionReferenceNumber")?.Value ?? string.Empty,
            OriginalTrackingNumber = transaction?.Element("OriginalTransactionNumber")?.Value,
            ServicePointIdentifier = transaction?.Element("UtilityAccountNumber")?.Value ?? string.Empty,
        };
    }
}
