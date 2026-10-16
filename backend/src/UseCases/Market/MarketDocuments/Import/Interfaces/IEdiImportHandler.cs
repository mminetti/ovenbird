using Core.MarketExchange;
using Core.Settings;

namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public interface IEdiImportHandler
{
    string Identifier { get; }

    Task<IReadOnlyList<string>> ListTransactionsAsync(Configuration configuration, CancellationToken ct);

    Task<MarketDocument?> FindExistingTransactionAsync(Configuration configuration, string reference, CancellationToken ct);

    Task<TransactionReference> StoreTransactionAsync(Configuration configuration, string reference, CancellationToken ct);

    Task<Stream> OpenTransactionAsync(Configuration configuration, string fileReference, CancellationToken ct);
}
