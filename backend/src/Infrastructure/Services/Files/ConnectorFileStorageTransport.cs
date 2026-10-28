using Core.Settings;
using UseCases.Interfaces.Files;
using UseCases.Interfaces.Secrets;
using Core.Common.Constants;

namespace Infrastructure.Services.Files;

public class ConnectorFileStorageTransport(
    IServiceProvider serviceProvider,
    IConnectorFieldSecretResolver secretResolver,
    TimeProvider timeProvider)
    : IConnectorFileStorageTransport
{
    private const string ConnectorFileStorageConnectionString = "connection.string";
    private const string ConfigurationFileStorageRootDirectory = "file.storage.root.directory";
    private const string ConfigurationFileStorageFolder = "file.storage.folder";

    public async Task<string> UploadAsync(Configuration configuration, Stream content, string fileName, CancellationToken ct)
    {
        var fileStorage = await ResolveFileStorage(configuration, ct);

        return await fileStorage.Service.UploadAsync(fileStorage.Options, content, fileName, ct);
    }

    public async Task<Stream> OpenReadAsync(Configuration configuration, string fileReference, CancellationToken ct)
    {
        var fileStorage = await ResolveFileStorage(configuration, ct);

        return await fileStorage.Service.OpenReadAsync(fileStorage.Options, fileReference, ct);
    }

    private record FileStorageContext(FileStorageOptions Options, IFileStorage Service);

    private async Task<FileStorageContext> ResolveFileStorage(Configuration configuration, CancellationToken ct)
    {
        var options = await ResolveFileOptions(configuration, ct);
        var service = KeyedServiceResolver.Resolve<IFileStorage>(serviceProvider, configuration.Name, options.Implementation);

        return new FileStorageContext(options, service);
    }

    private async Task<FileStorageOptions> ResolveFileOptions(Configuration configuration, CancellationToken ct)
    {
        var connector = configuration.GetRequiredConnector(Constants.ConnectorTypes.FileStorage);

        var now = configuration.GetRequiredCompany().GetLocalNow(timeProvider);

        return new FileStorageOptions
        {
            RootDirectory = configuration.GetRequiredValue(ConfigurationFileStorageRootDirectory),
            FileFolder = $"{configuration.GetRequiredValue(ConfigurationFileStorageFolder)}/{now:yyyy/MM/dd}",
            ConnectionString = await connector.GetRequiredValueAsync(ConnectorFileStorageConnectionString, secretResolver, ct),
            Implementation = connector.ConnectorImplementation.Identifier,
        };
    }
}
