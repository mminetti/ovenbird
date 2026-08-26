using Core.Shared;
using UseCases.Interfaces.Files;

namespace UseCases.Market.MarketDocuments.Import.Strategies;

public class BigDataImportStrategy(
    ISftpService sftpService,
    IFtpService ftpService,
    IFileStorage fileStorage) : IMarketImportStrategy
{
    public const string Identifier = "BigData";

    private const string HostFieldIdentifier = "Host";
    private const string PortFieldIdentifier = "Port";
    private const string UsernameFieldIdentifier = "Username";
    private const string PasswordFieldIdentifier = "Password";
    private const string RemoteDirectory = "RemoteDirectory";
    private const string RootDirectory = "RootDirectory";
    private const string FtpType = "FtpType";

    public string MarketIdentifier => Identifier;

    public Task<IReadOnlyList<string>> ListFilesAsync(SystemIntegration integration, CancellationToken ct)
    {
        var options = ResolveFtpOptions(integration);

        if (options.Type == "SFTP")
        {
            return sftpService.ListAsync(options, ct);
        }
        else
        {
            return ftpService.ListAsync(options, ct);
        }
    }

    public Task<Stream> DownloadFileAsync(SystemIntegration integration, string remoteFilePath, CancellationToken ct)
    {
        var options = ResolveFtpOptions(integration);

        if (options.Type == "SFTP")
        {
            return sftpService.DownloadAsync(options, remoteFilePath, ct);
        }
        else
        {
            return ftpService.DownloadAsync(options, remoteFilePath, ct);
        }
    }

    public Task UploadFileAsync(SystemIntegration integration, Stream content, string remoteFilePath, CancellationToken ct)
    {
        var options = ResolveFtpOptions(integration);

        if (options.Type == "SFTP")
        {
            return sftpService.UploadAsync(options, content, remoteFilePath, ct);
        }
        else
        {
            return ftpService.UploadAsync(options, content, remoteFilePath, ct);
        }
    }

    public Task<string> UploadDocumentAsync(SystemIntegration integration, Stream content, string remoteFilePath, CancellationToken ct)
    {
        var options = ResolveFileOptions(integration);

        return fileStorage.UploadAsync(options, content, remoteFilePath, ct);
    }

    private static FtpOptions ResolveFtpOptions(SystemIntegration integration)
    {
        return new FtpOptions
        {
            Host = integration.GetRequiredValue(HostFieldIdentifier),
            Port = int.TryParse(integration.GetValue(PortFieldIdentifier), out var port) ? port : 22,
            Username = integration.GetRequiredValue(UsernameFieldIdentifier),
            Password = integration.GetRequiredValue(PasswordFieldIdentifier),
            RemoteDirectory = integration.GetRequiredValue(RemoteDirectory),
            Type = integration.GetRequiredValue(FtpType),
        };
    }

    private static Interfaces.Files.FileOptions ResolveFileOptions(SystemIntegration integration)
    {
        return new Interfaces.Files.FileOptions
        {
            RootDirectory = integration.GetRequiredValue(RootDirectory),
        };
    }
}
