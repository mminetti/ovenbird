using Core.Constants;
using Core.Shared;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Interfaces.Files;

namespace UseCases.Market.MarketDocuments.Import.Strategies;

public class BigDataImportStrategy(IServiceProvider serviceProvider) : IMarketImportStrategy
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

    public Task<IReadOnlyList<string>> ListFilesAsync(Configuration configuration, CancellationToken ct)
    {
        var ftp = ResolveFtp(configuration);

        return ftp.Service.ListAsync(ftp.Options, ct);
    }

    public Task<Stream> DownloadFileAsync(Configuration configuration, string remoteFilePath, CancellationToken ct)
    {
        var ftp = ResolveFtp(configuration);

        return ftp.Service.DownloadAsync(ftp.Options, remoteFilePath, ct);
    }

    public Task<string> UploadDocumentAsync(Configuration configuration, Stream content, string remoteFilePath, CancellationToken ct)
    {
        var fileStorage = ResolveFileStorage(configuration);

        return fileStorage.Service.UploadAsync(fileStorage.Options, content, remoteFilePath, ct);
    }

    private record FtpContext(FtpOptions Options, IFtpService Service);

    private record FileStorageContext(FileStorageOptions Options, IFileStorage Service);

    private FtpContext ResolveFtp(Configuration configuration)
    {
        var options = ResolveFtpOptions(configuration);
        var service = ResolveService<IFtpService>(configuration.Name, options.Implementation);

        return new FtpContext(options, service);
    }

    private FileStorageContext ResolveFileStorage(Configuration configuration)
    {
        var options = ResolveFileOptions(configuration);
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

    private static FtpOptions ResolveFtpOptions(Configuration configuration)
    {
        var connector = configuration.GetRequiredConnector(ConnectorTypes.Ftp);

        return new FtpOptions
        {
            Host = connector.GetRequiredValue(FtpHost),
            Port = int.TryParse(connector.GetRequiredValue(FtpPort), out var port) ? port : DefaultFtpPort,
            Username = connector.GetRequiredValue(FtpUsername),
            Password = connector.GetRequiredValue(FtpPassword),
            RemoteDirectory = configuration.GetRequiredValue(FtpRemoteDirectory),
            Implementation = connector.ConnectorImplementation.Name,
        };
    }

    private static FileStorageOptions ResolveFileOptions(Configuration configuration)
    {
        var connector = configuration.GetRequiredConnector(ConnectorTypes.FileStorage);

        return new FileStorageOptions
        {
            RootDirectory = connector.GetRequiredValue(FileStorageRootDirectory),
            ConnectionString = connector.GetRequiredValue(FileStorageConnectionString),
            Implementation = connector.ConnectorImplementation.Name,
        };
    }
}
