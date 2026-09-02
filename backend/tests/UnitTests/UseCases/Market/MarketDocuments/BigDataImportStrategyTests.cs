using Core.Constants;
using Core.Shared;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Interfaces.Files;
using UseCases.Market.MarketDocuments.Import.Strategies;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class BigDataImportStrategyTests
{
    private const string FtpImplementation = "TestFtpImplementation";

    private readonly IFtpService _ftpService = Substitute.For<IFtpService>();
    private readonly BigDataImportStrategy _strategy;

    public BigDataImportStrategyTests()
    {
        var services = new ServiceCollection();
        services.AddKeyedSingleton(FtpImplementation, _ftpService);

        _strategy = new BigDataImportStrategy(services.BuildServiceProvider());
    }

    [Fact]
    public async Task ListFilesAsyncDelegatesToSftpServiceUsingIntegrationFields()
    {
        var configuration = CreateConfiguration(
            CreateSftpIntegration("sftp.example.com", "2222", "user", "pass"),
            remoteDirectory: "remote/dir");

        _ftpService.ListAsync(
                Arg.Is<FtpOptions>(o =>
                    o.Host == "sftp.example.com" && o.Port == 2222 && o.Username == "user" &&
                    o.Password == "pass" && o.RemoteDirectory == "remote/dir"),
                Arg.Any<CancellationToken>())
            .Returns(["remote/dir/file1.csv"]);

        var files = await _strategy.ListFilesAsync(configuration, CancellationToken.None);

        files.ShouldBe(["remote/dir/file1.csv"]);
    }

    [Fact]
    public async Task ListFilesAsyncDefaultsPortWhenFieldMissing()
    {
        var configuration = CreateConfiguration(
            CreateSftpIntegration("sftp.example.com", port: null, "user", "pass"),
            remoteDirectory: "remote/dir");

        _ftpService.ListAsync(Arg.Is<FtpOptions>(o => o.Port == 22), Arg.Any<CancellationToken>())
            .Returns([]);

        await _strategy.ListFilesAsync(configuration, CancellationToken.None);

        await _ftpService.Received(1).ListAsync(Arg.Is<FtpOptions>(o => o.Port == 22), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ListFilesAsyncThrowsWhenRequiredFieldIsMissing()
    {
        var configuration = CreateConfiguration(
            CreateSftpIntegration(host: null, "22", "user", "pass"),
            remoteDirectory: "remote/dir");

        await Should.ThrowAsync<InvalidOperationException>(
            () => _strategy.ListFilesAsync(configuration, CancellationToken.None));
    }

    [Fact]
    public async Task ListFilesAsyncThrowsWhenRemoteDirectoryIsMissing()
    {
        var configuration = CreateConfiguration(
            CreateSftpIntegration("sftp.example.com", "22", "user", "pass"),
            remoteDirectory: null);

        await Should.ThrowAsync<InvalidOperationException>(
            () => _strategy.ListFilesAsync(configuration, CancellationToken.None));
    }

    [Fact]
    public async Task ListFilesAsyncThrowsWhenFtpIntegrationIsMissing()
    {
        var configuration = new Configuration { Id = 1, Name = "edi.import" };

        await Should.ThrowAsync<InvalidOperationException>(
            () => _strategy.ListFilesAsync(configuration, CancellationToken.None));
    }

    private static Configuration CreateConfiguration(Connector ftpIntegration, string? remoteDirectory)
    {
        var fields = new List<ConfigurationField>();

        if (remoteDirectory is not null)
        {
            fields.Add(new ConfigurationField { ConfigurationId = 1, Name = "ftp.remote.directory", Value = remoteDirectory });
        }

        return new Configuration
        {
            Id = 1,
            Name = "edi.import",
            Connectors = [ftpIntegration],
            ConfigurationFields = fields,
        };
    }

    private static Connector CreateSftpIntegration(
        string? host, string? port, string? username, string? password)
    {
        var integration = new Connector
        {
            Id = 1,
            Name = "Sftp",
            ConnectorType = new ConnectorType { Id = 1, Name = ConnectorTypes.Ftp },
            ConnectorImplementation = new ConnectorImplementation { Id = 1, Name = FtpImplementation },
        };

        var fields = new List<ConnectorField>();

        void AddField(string identifier, string? value)
        {
            if (value is not null)
            {
                fields.Add(new ConnectorField { ConnectorId = integration.Id, Name = identifier, Value = value });
            }
        }

        AddField("host", host);
        AddField("port", port);
        AddField("username", username);
        AddField("password", password);

        integration.ConnectorFields = fields;

        return integration;
    }
}
