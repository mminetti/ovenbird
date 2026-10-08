using System.Xml.Linq;
using Core.Market;

namespace UseCases.Market.MarketDocuments.Import.BigData.Readers;

public class InvoiceTransactionReader : IMarketDocumentTransactionReader
{
    public const string Identifier = "810_02";

    public MarketDocumentItem Read(XElement transactionElement)
    {
        var transaction = transactionElement.Element("Invoice");

        var billPurpose = transaction?.Element("BillPurpose")?.Value ?? string.Empty;
        var purpose = billPurpose switch
        {
            "01" => "CANCEL",
            "05" => "REPLACE",
            "07" => "DUPLICATE",
            _ => "ORIGINAL",
        };

        var billActionCode = transaction?.Element("BillActionCode")?.Value ?? string.Empty;
        var subPurpose = billActionCode switch
        {
            "26" => "CHARGES",
            "FE" or "FB" => "FINAL",
            "A5" => "TAMPERING",
            "BD" => "BALANCE_DUE",
            _ => "MONTH",
        };

        var fields = transaction?.Flatten("Invoice") ?? [];

        return new MarketDocumentItem
        {
            Purpose = purpose,
            SubPurpose = subPurpose,
            TrackingNumber = transaction?.Element("BillNumber")?.Value ?? string.Empty,
            OriginalTrackingNumber = transaction?.Element("OriginalBillNumber")?.Value,
            ServicePointIdentifier = transaction?.Element("LDCAccountNumber")?.Value ?? string.Empty,
            Fields = [.. fields.Where(x => !string.IsNullOrWhiteSpace(x.Value))
                .Select(x => new MarketDocumentItemField
                {
                    FieldName = x.Name,
                    FieldValue = x.Value
                })],
        };
    }
}
