using System.Xml.Linq;
using Core.Market;
using Core.Settings;
using Infrastructure.Services.Market.BigData.Readers;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Interfaces.Files;
using UseCases.Interfaces.Secrets;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.Market.BigData;

public class BigDataMarketDocumentImport(
    IServiceProvider serviceProvider,
    IConnectorFieldSecretResolver secretResolver,
    TimeProvider timeProvider)
    : IMarketDocumentImport
{
    private const int DefaultFtpPort = 22;

    private const string ConnectorFtpHost = "host";
    private const string ConnectorFtpPort = "port";
    private const string ConnectorFtpUsername = "username";
    private const string ConnectorFtpPassword = "password";
    private const string ConnectorFileStorageConnectionString = "connection.string";
    private const string ConfigurationFtpRootDirectory = "ftp.root.directory";
    private const string ConfigurationFileStorageRootDirectory = "file.storage.root.directory";
    private const string ConfigurationFileStorageFolder = "file.storage.folder";

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

    public async Task<string> UploadDocumentAsync(Configuration configuration, Stream content, string fileName, CancellationToken ct)
    {
        var fileStorage = await ResolveFileStorage(configuration, ct);

        return await fileStorage.Service.UploadAsync(fileStorage.Options, content, fileName, ct);
    }

    public async Task<Stream> OpenDocumentAsync(Configuration configuration, string fileReference, CancellationToken ct)
    {
        var fileStorage = await ResolveFileStorage(configuration, ct);

        return await fileStorage.Service.OpenReadAsync(fileStorage.Options, fileReference, ct);
    }

    public async Task<IReadOnlyList<MarketDocumentItem>> ReadTransactionsAsync(Stream content, CancellationToken ct)
    {
        var document = await XDocument.LoadAsync(content, LoadOptions.None, ct);

        var transactions = document.Root?.Element("TransactionList")?.Elements("Transaction") ?? [];

        var items = new List<MarketDocumentItem>();

        foreach (var transaction in transactions)
        {
            var set = transaction.Element("TransactionSet")?.Value ?? string.Empty;
            var subSet = transaction.Element("TransactionSubSet")?.Value ?? string.Empty;

            var reader = ResolveReader($"{set}_{subSet}");
            var item = reader.Read(transaction);

            item.Set = set;
            item.SubSet = subSet;
            item.Raw = transaction.ToString();
            item.MarketDocumentItemStatusId = CoreConstants.MarketDocumentItemStatuses.New;

            items.Add(item);
        }

        return items;
    }

    private IMarketDocumentTransactionReader ResolveReader(string set)
    {
        try
        {
            return serviceProvider.GetRequiredKeyedService<IMarketDocumentTransactionReader>(set);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException(
                $"Market Document Transaction Reader couldn't resolve TransactionSet '{set}'.", ex);
        }
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
        var connector = configuration.GetRequiredConnector(CoreConstants.ConnectorTypes.Ftp);

        return new FtpOptions
        {
            Host = await connector.GetRequiredValueAsync(ConnectorFtpHost, secretResolver, ct),
            Port = int.TryParse(connector.GetValue(ConnectorFtpPort), out var port) ? port : DefaultFtpPort,
            Username = await connector.GetRequiredValueAsync(ConnectorFtpUsername, secretResolver, ct),
            Password = await connector.GetRequiredValueAsync(ConnectorFtpPassword, secretResolver, ct),
            RootDirectory = configuration.GetRequiredValue(ConfigurationFtpRootDirectory),
            Implementation = connector.ConnectorImplementation.Identifier,
        };
    }

    private async Task<FileStorageOptions> ResolveFileOptions(Configuration configuration, CancellationToken ct)
    {
        var connector = configuration.GetRequiredConnector(CoreConstants.ConnectorTypes.FileStorage);

        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(configuration.GetRequiredCompany().TimeZoneId);

        var now = TimeZoneInfo.ConvertTime(timeProvider.GetUtcNow(), timeZone);

        return new FileStorageOptions
        {
            RootDirectory = configuration.GetRequiredValue(ConfigurationFileStorageRootDirectory),
            FileFolder = $"{configuration.GetRequiredValue(ConfigurationFileStorageFolder)}/{now:yyyy/MM/dd}",
            ConnectionString = await connector.GetRequiredValueAsync(ConnectorFileStorageConnectionString, secretResolver, ct),
            Implementation = connector.ConnectorImplementation.Identifier,
        };
    }
}
