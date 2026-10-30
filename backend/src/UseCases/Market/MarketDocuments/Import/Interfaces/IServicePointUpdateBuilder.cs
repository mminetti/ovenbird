using Core.MarketExchange;
using UseCases.Interfaces.ServiceDelivery;

namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public interface IServicePointUpdateBuilder
{
    Task<Result<UpdateServicePointRequest>> BuildAsync(MarketDocumentItem item, CancellationToken ct);
}
