using System.Xml.Linq;
using Core.Market;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Market.MarketDocuments.Import.BigData.Readers;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace UseCases.Market.MarketDocuments.Import.BigData;

public class BigDataMarketDocumentItemProcessor(IServiceProvider serviceProvider) : IMarketDocumentItemProcessor
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

            var reader = ResolveReader(set);
            var item = reader.Read(transaction);

            item.Set = set;
            item.SubSet = subSet;
            item.Raw = transaction.ToString();
            item.MarketDocumentItemStatusId = CoreConstants.MarketDocumentItemStatuses.New;

            items.Add(item);
        }

        return items;
    }

    private IMarketDocumentTransactionReader ResolveReader(string set)
    {
        try
        {
            return serviceProvider.GetRequiredKeyedService<IMarketDocumentTransactionReader>(set);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException(
                $"Market Document Transaction Reader couldn't resolve TransactionSet '{set}'.", ex);
        }
    }
}
