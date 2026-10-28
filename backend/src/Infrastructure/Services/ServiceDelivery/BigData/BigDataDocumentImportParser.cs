using System.Xml.Linq;
using Core.MarketExchange;
using Infrastructure.Services.ServiceDelivery.BigData.Readers;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.ServiceDelivery.BigData;

public class BigDataDocumentImportParser(IServiceProvider serviceProvider) : IMarketDocumentParser
{
    public string Identifier => "BigData";

    public async Task<IReadOnlyList<MarketDocumentItem>> ReadTransactionsAsync(Stream content, CancellationToken ct)
    {
        var document = await XDocument.LoadAsync(content, LoadOptions.None, ct);

        var transactions = document.Root?.Element("TransactionList")?.Elements("Transaction") ?? [];

        var items = new List<MarketDocumentItem>();

        foreach (var transaction in transactions)
        {
            string set = transaction.Element("TransactionSet")?.Value!;
            string subSet = transaction.Element("TransactionSubSet")?.Value!;

            var reader = GetRequiredReader(set);
            var item = reader.Read(transaction);

            item.Set = set;
            item.SubSet = subSet;
            item.Raw = transaction.ToString();
            item.MarketDocumentItemStatusId = CoreConstants.MarketDocumentItemStatuses.New;

            items.Add(item);
        }

        return items;
    }

    private IMarketDocumentTransactionReader GetRequiredReader(string set)
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
