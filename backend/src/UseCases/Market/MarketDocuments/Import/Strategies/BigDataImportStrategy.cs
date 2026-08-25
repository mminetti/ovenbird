using Core.Shared;
using UseCases.Interfaces.Files;

namespace UseCases.Market.MarketDocuments.Import.Strategies;

public class BigDataImportStrategy(ISftpService sftpService, IFileStorage fileStorage) : IMarketImportStrategy
{
    public const string Identifier = "BigData";

    private const string HostFieldIdentifier = "Host";
    private const string PortFieldIdentifier = "Port";
    private const string UsernameFieldIdentifier = "Username";
    private const string PasswordFieldIdentifier = "Password";
    private const string RemoteDirectory = "RemoteDirectory";

    public string MarketIdentifier => Identifier;

    public Task<IReadOnlyList<string>> ListFilesAsync(SystemIntegration integration, CancellationToken ct)
    {
        //TODO: implementation should be one or the other based on options

        return sftpService.ListAsync(ResolveSftpOptions(integration), ct);
    }

    public Task<Stream> DownloadFileAsync(SystemIntegration integration, string remoteFilePath, CancellationToken ct)
    {
        return sftpService.DownloadAsync(ResolveSftpOptions(integration), remoteFilePath, ct);
    }

    public Task UploadFileAsync(SystemIntegration integration, Stream content, string remoteFilePath, CancellationToken ct)
    {
        return sftpService.UploadAsync(ResolveSftpOptions(integration), content, remoteFilePath, ct);
    }

    public Task<string> UploadDocumentAsync(SystemIntegration integration, Stream content, string remoteFilePath, CancellationToken ct)
    {
        return fileStorage.UploadAsync(content, remoteFilePath, ct);
    }

    private static SftpOptions ResolveSftpOptions(SystemIntegration integration)
    {
        return new SftpOptions
        {
            Host = integration.GetRequiredValue(HostFieldIdentifier),
            Port = int.TryParse(integration.GetValue(PortFieldIdentifier), out var port) ? port : 22,
            Username = integration.GetRequiredValue(UsernameFieldIdentifier),
            Password = integration.GetRequiredValue(PasswordFieldIdentifier),
            RemoteDirectory = integration.GetRequiredValue(RemoteDirectory),
        };
    }
}
