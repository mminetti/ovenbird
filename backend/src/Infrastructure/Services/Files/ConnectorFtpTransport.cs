using Core.Settings;
using UseCases.Interfaces.Files;
using UseCases.Interfaces.Secrets;
using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.Files;

public class ConnectorFtpTransport(
    IServiceProvider serviceProvider,
    IConnectorFieldSecretResolver secretResolver)
    : IConnectorFtpTransport
{
    private const int DefaultFtpPort = 22;

    private const string ConnectorFtpHost = "host";
    private const string ConnectorFtpPort = "port";
    private const string ConnectorFtpUsername = "username";
    private const string ConnectorFtpPassword = "password";
    private const string ConfigurationFtpRootDirectory = "ftp.root.directory";

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

    public async Task UploadFileAsync(Configuration configuration, Stream content, string remoteFileName, CancellationToken ct)
    {
        var ftp = await ResolveFtp(configuration, ct);

        await ftp.Service.UploadAsync(ftp.Options, content, $"{ftp.Options.RootDirectory}/{remoteFileName}", ct);
    }

    private record FtpContext(FtpOptions Options, IFtpService Service);

    private async Task<FtpContext> ResolveFtp(Configuration configuration, CancellationToken ct)
    {
        var options = await ResolveFtpOptions(configuration, ct);
        var service = KeyedServiceResolver.Resolve<IFtpService>(serviceProvider, configuration.Name, options.Implementation);

        return new FtpContext(options, service);
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
}
