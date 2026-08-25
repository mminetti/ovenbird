using Core.Shared;

namespace UseCases.Market.MarketDocuments;

public interface IMarketConnectionStrategy
{
    string MarketIdentifier { get; }

    Task<IReadOnlyList<string>> ListFilesAsync(Company company, string remoteDirectory, CancellationToken ct);

    Task<Stream> DownloadFileAsync(Company company, string remoteFilePath, CancellationToken ct);

    Task UploadFileAsync(Company company, Stream content, string remoteFilePath, CancellationToken ct);
}
