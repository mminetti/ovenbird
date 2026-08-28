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
        var (options, ftpService) = ResolveFtp(configuration);

        return ftpService.ListAsync(options, ct);
    }

    public Task<Stream> DownloadFileAsync(Configuration configuration, string remoteFilePath, CancellationToken ct)
    {
        var (options, ftpService) = ResolveFtp(configuration);

        return ftpService.DownloadAsync(options, remoteFilePath, ct);
    }

    public Task<string> UploadDocumentAsync(Configuration configuration, Stream content, string remoteFilePath, CancellationToken ct)
    {
        var (options, fileStorage) = ResolveFileStorage(configuration);

        return fileStorage.UploadAsync(options, content, remoteFilePath, ct);
    }

    private (FtpOptions Options, IFtpService Service) ResolveFtp(Configuration configuration)
    {
        var options = ResolveFtpOptions(configuration);
        var service = ResolveService<IFtpService>(configuration.Name, options.Implementation);

        return (options, service);
    }

    private (FileStorageOptions Options, IFileStorage Service) ResolveFileStorage(Configuration configuration)
    {
        var options = ResolveFileOptions(configuration);
        var service = ResolveService<IFileStorage>(configuration.Name, options.Implementation);

        return (options, service);
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
                $"Configuration '{configurationName}' requested {typeof(TService).Name} implementation '{implementation}' which isn't registered.",
                ex);
        }
    }

    private static FtpOptions ResolveFtpOptions(Configuration configuration)
    {
        var connector = configuration.GetRequiredConnector(ConnectorTypes.Ftp);

        return new FtpOptions
        {
            Host = connector.GetRequiredValue(FtpHost),
            Port = ResolvePort(connector),
            Username = connector.GetRequiredValue(FtpUsername),
            Password = connector.GetRequiredValue(FtpPassword),
            RemoteDirectory = configuration.GetRequiredValue(FtpRemoteDirectory),
            Implementation = connector.ConnectorImplementation.Name,
        };
    }

    private static int ResolvePort(Connector connector)
    {
        var value = connector.GetValue(FtpPort);

        if (string.IsNullOrWhiteSpace(value))
        {
            return DefaultFtpPort;
        }

        if (!int.TryParse(value, out var port))
        {
            throw new InvalidOperationException(
                $"Connector '{connector.Name}' has an invalid '{FtpPort}' value '{value}'.");
        }

        return port;
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
