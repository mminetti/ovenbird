using Core.MarketExchange;

namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public interface IMarketDocumentEntityBuilder
{
    int EventTypeId { get; }

    Task<Result<IReadOnlyList<long>>> CreateAsync(MarketDocumentItem item, CancellationToken ct);
}
