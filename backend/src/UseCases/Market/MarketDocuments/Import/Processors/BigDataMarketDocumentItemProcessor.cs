using System.Xml.Linq;
using Core.Market;
using UseCases.Market.MarketDocuments.Import.Processors.Readers;
using CoreConstants = Core.Common.Constants.Constants;

namespace UseCases.Market.MarketDocuments.Import.Processors;

public class BigDataMarketDocumentItemProcessor(MarketDocumentTransactionReaderResolver readerResolver)
    : IMarketDocumentItemProcessor
{
    public string Identifier => "BigData";

    public async Task<IReadOnlyList<MarketDocumentItem>> ProcessAsync(Stream content, CancellationToken ct)
    {
        var document = await XDocument.LoadAsync(content, LoadOptions.None, ct);

        var transactions = document.Root?.Element("TransactionList")?.Elements("Transaction") ?? [];

        var items = new List<MarketDocumentItem>();

        foreach (var transaction in transactions)
        {
            var set = transaction.Element("TransactionSet")?.Value ?? string.Empty;
            var subSet = transaction.Element("TransactionSubSet")?.Value ?? string.Empty;

            var reader = readerResolver.Resolve(set);
            var item = reader.Read(transaction);

            item.Set = set;
            item.SubSet = subSet;
            item.Raw = transaction.ToString();
            item.MarketDocumentItemStatusId = CoreConstants.MarketDocumentItemStatuses.New;

            items.Add(item);
        }

        return items;
    }
}
