using Core.Shared;
using UseCases.Interfaces.Files;

namespace UseCases.Market.MarketDocuments.Import.Strategies;

public class BigDataImportStrategy(
    ISftpService sftpService,
    IFtpService ftpService,
    IFileStorage fileStorage) : IMarketImportStrategy
{
    public const string Identifier = "BigData";

    private const string FtpImplementation = "Ftp";
    private const string FileStorageImplementation = "FileStorage";

    private const string HostFieldIdentifier = "Host";
    private const string PortFieldIdentifier = "Port";
    private const string UsernameFieldIdentifier = "Username";
    private const string PasswordFieldIdentifier = "Password";
    private const string RemoteDirectory = "RemoteDirectory";
    private const string RootDirectory = "RootDirectory";
    private const string FtpType = "FtpType";

    public string MarketIdentifier => Identifier;

    public Task<IReadOnlyList<string>> ListFilesAsync(Configuration configuration, CancellationToken ct)
    {
        var options = ResolveFtpOptions(configuration);

        if (options.Type == "SFTP")
        {
            return sftpService.ListAsync(options, ct);
        }
        else
        {
            return ftpService.ListAsync(options, ct);
        }
    }

    public Task<Stream> DownloadFileAsync(Configuration configuration, string remoteFilePath, CancellationToken ct)
    {
        var options = ResolveFtpOptions(configuration);

        if (options.Type == "SFTP")
        {
            return sftpService.DownloadAsync(options, remoteFilePath, ct);
        }
        else
        {
            return ftpService.DownloadAsync(options, remoteFilePath, ct);
        }
    }

    public Task UploadFileAsync(Configuration configuration, Stream content, string remoteFilePath, CancellationToken ct)
    {
        var options = ResolveFtpOptions(configuration);

        if (options.Type == "SFTP")
        {
            return sftpService.UploadAsync(options, content, remoteFilePath, ct);
        }
        else
        {
            return ftpService.UploadAsync(options, content, remoteFilePath, ct);
        }
    }

    public Task<string> UploadDocumentAsync(Configuration configuration, Stream content, string remoteFilePath, CancellationToken ct)
    {
        var options = ResolveFileOptions(configuration);

        return fileStorage.UploadAsync(options, content, remoteFilePath, ct);
    }

    private static FtpOptions ResolveFtpOptions(Configuration configuration)
    {
        var connector = configuration.GetRequiredConnector(FtpImplementation);

        return new FtpOptions
        {
            Host = connector.GetRequiredValue(HostFieldIdentifier),
            Port = int.TryParse(connector.GetValue(PortFieldIdentifier), out var port) ? port : 22,
            Username = connector.GetRequiredValue(UsernameFieldIdentifier),
            Password = connector.GetRequiredValue(PasswordFieldIdentifier),
            RemoteDirectory = connector.GetRequiredValue(RemoteDirectory),
            Type = connector.GetRequiredValue(FtpType),
        };
    }

    private static Interfaces.Files.FileOptions ResolveFileOptions(Configuration configuration)
    {
        var connector = configuration.GetRequiredConnector(FileStorageImplementation);

        return new Interfaces.Files.FileOptions
        {
            RootDirectory = connector.GetRequiredValue(RootDirectory),
        };
    }
}
