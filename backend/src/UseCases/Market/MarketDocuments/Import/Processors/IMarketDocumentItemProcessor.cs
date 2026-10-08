using Core.Market;

namespace UseCases.Market.MarketDocuments.Import.Processors;

public interface IMarketDocumentItemProcessor
{
    string Identifier { get; }
    Task<IReadOnlyList<MarketDocumentItem>> ProcessAsync(Stream content, CancellationToken ct);
}
