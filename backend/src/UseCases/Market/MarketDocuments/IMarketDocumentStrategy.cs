using Core.Shared;

namespace UseCases.Market.MarketDocuments;

public interface IMarketDocumentStrategy
{
    string MarketIdentifier { get; }

    Task<IReadOnlyList<string>> ListFilesAsync(Company company, string remoteDirectory, CancellationToken ct);

    Task<Stream> DownloadAsync(Company company, string remoteFilePath, CancellationToken ct);

    Task UploadAsync(string remoteFilePath, CancellationToken ct);
}
