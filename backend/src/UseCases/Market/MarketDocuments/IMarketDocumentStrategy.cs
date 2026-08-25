using Core.Shared;

namespace UseCases.Market.MarketDocuments;

public interface IMarketDocumentStrategy
{
    string MarketIdentifier { get; }

    Task<IReadOnlyList<string>> ListFilesAsync(Company company, string remoteDirectory, CancellationToken ct);

    Task<Stream> DownloadFileAsync(Company company, string remoteFilePath, CancellationToken ct);

    Task UploadFileAsync(string remoteFilePath, CancellationToken ct);
}
