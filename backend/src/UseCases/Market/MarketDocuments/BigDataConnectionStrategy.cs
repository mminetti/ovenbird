using Core.Shared;
using UseCases.Interfaces.Files;

namespace UseCases.Market.MarketDocuments;

public class BigDataConnectionStrategy(ISftpService sftpService) : IMarketConnectionStrategy
{
    public const string Identifier = "BigData";

    public string MarketIdentifier => Identifier;

    public Task<IReadOnlyList<string>> ListFilesAsync(Company company, string remoteDirectory, CancellationToken ct)
    {
        return sftpService.ListAsync(remoteDirectory, ct);
    }

    public Task<Stream> DownloadFileAsync(Company company, string remoteFilePath, CancellationToken ct)
    {
        return sftpService.DownloadAsync(remoteFilePath, ct);
    }

    public Task UploadFileAsync(Stream content, string remoteFilePath, CancellationToken ct)
    {
        return sftpService.UploadAsync(content, remoteFilePath, ct);
    }
}
