using Core.MarketExchange;

namespace UseCases.Market.MarketDocuments.Export.Interfaces;

public interface IMarketDocumentGenerator
{
    string Identifier { get; }
    Task<Stream> WriteTransactionsAsync(IReadOnlyList<MarketDocumentItem> items, CancellationToken ct);
}
