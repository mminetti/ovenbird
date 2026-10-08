using System.Xml.Linq;
using Core.Market;

namespace UseCases.Market.MarketDocuments.Import.BigData.Readers;

public class InvoiceTransactionReader : IMarketDocumentTransactionReader
{
    public const string Identifier = "810_02";

    public MarketDocumentItem Read(XElement transactionElement)
    {
        var invoice = transactionElement.Element("Invoice");

        var billPurpose = invoice?.Element("BillPurpose")?.Value ?? string.Empty;
        var purpose = billPurpose switch
        {
            "01" => "CANCEL",
            "05" => "REPLACE",
            "07" => "DUPLICATE",
            _ => "ORIGINAL",
        };

        var billActionCode = invoice?.Element("BillActionCode")?.Value ?? string.Empty;
        var subPurpose = billActionCode switch
        {
            "26" => "CHARGES",
            "FE" or "FB" => "FINAL",
            "A5" => "TAMPERING",
            "BD" => "BALANCE_DUE",
            _ => "MONTH",
        };

        return new MarketDocumentItem
        {
            Purpose = purpose,
            SubPurpose = subPurpose,
            TrackingNumber = invoice?.Element("BillNumber")?.Value ?? string.Empty,
            OriginalTrackingNumber = invoice?.Element("OriginalBillNumber")?.Value,
            ServicePointIdentifier = invoice?.Element("LDCAccountNumber")?.Value ?? string.Empty,
        };
    }
}
