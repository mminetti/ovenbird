using Core.MarketExchange;

namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public interface IMarketDocumentParser
{
    string Identifier { get; }
    Task<IReadOnlyList<MarketDocumentItem>> ReadTransactionsAsync(Stream content, CancellationToken ct);
}
