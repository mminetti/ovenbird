using Core.Constants;
using Core.Shared;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Interfaces.Files;
using UseCases.Interfaces.Secrets;

namespace UseCases.Market.MarketDocuments.Import.Strategies;

public class BigDataImportStrategy(IServiceProvider serviceProvider, IConnectorFieldSecretResolver secretResolver)
    : IMarketImportStrategy
{
    private const int DefaultFtpPort = 22;

    private const string FtpHost = "host";
    private const string FtpPort = "port";
    private const string FtpUsername = "username";
    private const string FtpPassword = "password";
    private const string FtpRemoteDirectory = "ftp.remote.directory";

    private const string FileStorageRootDirectory = "root.directory";
    private const string FileStorageConnectionString = "connection.string";

    public string Identifier => "BigData";

    public async Task<IReadOnlyList<string>> ListFilesAsync(Configuration configuration, CancellationToken ct)
    {
        var ftp = await ResolveFtp(configuration, ct);

        return await ftp.Service.ListAsync(ftp.Options, ct);
    }

    public async Task<Stream> DownloadFileAsync(Configuration configuration, string remoteFilePath, CancellationToken ct)
    {
        var ftp = await ResolveFtp(configuration, ct);

        return await ftp.Service.DownloadAsync(ftp.Options, remoteFilePath, ct);
    }

    public async Task<string> UploadDocumentAsync(Configuration configuration, Stream content, string remoteFilePath, CancellationToken ct)
    {
        var fileStorage = await ResolveFileStorage(configuration, ct);

        return await fileStorage.Service.UploadAsync(fileStorage.Options, content, remoteFilePath, ct);
    }

    private record FtpContext(FtpOptions Options, IFtpService Service);

    private record FileStorageContext(FileStorageOptions Options, IFileStorage Service);

    private async Task<FtpContext> ResolveFtp(Configuration configuration, CancellationToken ct)
    {
        var options = await ResolveFtpOptions(configuration, ct);
        var service = ResolveService<IFtpService>(configuration.Name, options.Implementation);

        return new FtpContext(options, service);
    }

    private async Task<FileStorageContext> ResolveFileStorage(Configuration configuration, CancellationToken ct)
    {
        var options = await ResolveFileOptions(configuration, ct);
        var service = ResolveService<IFileStorage>(configuration.Name, options.Implementation);

        return new FileStorageContext(options, service);
    }

    private TService ResolveService<TService>(string configurationName, string implementation)
        where TService : notnull
    {
        try
        {
            return serviceProvider.GetRequiredKeyedService<TService>(implementation);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException(
                $"Configuration '{configurationName}' implementation '{implementation}' is not registered.", ex);
        }
    }

    private async Task<FtpOptions> ResolveFtpOptions(Configuration configuration, CancellationToken ct)
    {
        var connector = configuration.GetRequiredConnector(ConnectorTypes.Ftp);

        return new FtpOptions
        {
            Host = connector.GetRequiredValue(FtpHost),
            Port = int.TryParse(connector.GetValue(FtpPort), out var port) ? port : DefaultFtpPort,
            Username = connector.GetRequiredValue(FtpUsername),
            Password = await connector.GetRequiredResolvedValueAsync(FtpPassword, secretResolver, ct),
            RemoteDirectory = configuration.GetRequiredValue(FtpRemoteDirectory),
            Implementation = connector.ConnectorImplementation.Name,
        };
    }

    private async Task<FileStorageOptions> ResolveFileOptions(Configuration configuration, CancellationToken ct)
    {
        var connector = configuration.GetRequiredConnector(ConnectorTypes.FileStorage);

        return new FileStorageOptions
        {
            RootDirectory = connector.GetRequiredValue(FileStorageRootDirectory),
            ConnectionString = await connector.GetRequiredResolvedValueAsync(FileStorageConnectionString, secretResolver, ct),
            Implementation = connector.ConnectorImplementation.Name,
        };
    }
}
