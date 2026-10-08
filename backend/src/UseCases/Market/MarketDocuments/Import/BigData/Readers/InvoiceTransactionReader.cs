using System.Xml.Linq;
using Core.Market;

namespace UseCases.Market.MarketDocuments.Import.BigData.Readers;

public class InvoiceTransactionReader : IMarketDocumentTransactionReader
{
    public const string TransactionSet = "810";

    public MarketDocumentItem Read(XElement transactionElement)
    {
        var invoice = transactionElement.Element("Invoice");

        return new MarketDocumentItem
        {
            Purpose = invoice?.Element("BillPurpose")?.Value ?? string.Empty,
            SubPurpose = invoice?.Element("BillActionCode")?.Value ?? string.Empty,
            TrackingNumber = invoice?.Element("BillNumber")?.Value ?? string.Empty,
            OriginalTrackingNumber = invoice?.Element("CrossRefNumber")?.Value,
            ServicePointIdentifier = invoice?.Element("LDCAccountNumber")?.Value ?? string.Empty,
        };
    }
}
