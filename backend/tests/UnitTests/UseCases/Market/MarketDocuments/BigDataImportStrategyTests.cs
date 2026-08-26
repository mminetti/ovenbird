using Core.Shared;
using UseCases.Interfaces.Files;
using UseCases.Market.MarketDocuments.Import.Strategies;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class BigDataImportStrategyTests
{
    private readonly ISftpService _sftpService = Substitute.For<ISftpService>();
    private readonly IFtpService _ftpService = Substitute.For<IFtpService>();
    private readonly IFileStorage _fileStorage = Substitute.For<IFileStorage>();
    private readonly BigDataImportStrategy _strategy;

    public BigDataImportStrategyTests()
    {
        _strategy = new BigDataImportStrategy(_sftpService, _ftpService, _fileStorage);
    }

    [Fact]
    public async Task ListFilesAsyncDelegatesToSftpServiceUsingIntegrationFields()
    {
        var integration = CreateSftpIntegration("sftp.example.com", "2222", "user", "pass", "remote/dir");

        _sftpService.ListAsync(
                Arg.Is<FtpOptions>(o =>
                    o.Host == "sftp.example.com" && o.Port == 2222 && o.Username == "user" &&
                    o.Password == "pass" && o.RemoteDirectory == "remote/dir"),
                Arg.Any<CancellationToken>())
            .Returns(["remote/dir/file1.csv"]);

        var files = await _strategy.ListFilesAsync(integration, CancellationToken.None);

        files.ShouldBe(["remote/dir/file1.csv"]);
    }

    [Fact]
    public async Task ListFilesAsyncDefaultsPortWhenFieldMissing()
    {
        var integration = CreateSftpIntegration("sftp.example.com", port: null, "user", "pass", "remote/dir");

        _sftpService.ListAsync(Arg.Is<FtpOptions>(o => o.Port == 22), Arg.Any<CancellationToken>())
            .Returns([]);

        await _strategy.ListFilesAsync(integration, CancellationToken.None);

        await _sftpService.Received(1).ListAsync(Arg.Is<FtpOptions>(o => o.Port == 22), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ListFilesAsyncThrowsWhenRequiredFieldIsMissing()
    {
        var integration = CreateSftpIntegration(host: null, "22", "user", "pass", "remote/dir");

        await Should.ThrowAsync<InvalidOperationException>(
            () => _strategy.ListFilesAsync(integration, CancellationToken.None));
    }

    [Fact]
    public async Task ListFilesAsyncThrowsWhenRemoteDirectoryIsMissing()
    {
        var integration = CreateSftpIntegration("sftp.example.com", "22", "user", "pass", remoteDirectory: null);

        await Should.ThrowAsync<InvalidOperationException>(
            () => _strategy.ListFilesAsync(integration, CancellationToken.None));
    }

    private static SystemIntegration CreateSftpIntegration(
        string? host, string? port, string? username, string? password, string? remoteDirectory)
    {
        var integration = new SystemIntegration { Id = 1, Identifier = "edi.import", Name = "Sftp" };

        var fields = new List<SystemIntegrationField>();

        void AddField(string identifier, string? value)
        {
            if (value is not null)
            {
                fields.Add(new SystemIntegrationField { SystemIntegrationId = integration.Id, Identifier = identifier, Name = identifier, Value = value });
            }
        }

        AddField("Host", host);
        AddField("Port", port);
        AddField("Username", username);
        AddField("Password", password);
        AddField("RemoteDirectory", remoteDirectory);
        AddField("FtpType", "SFTP");

        integration.SystemIntegrationFields = fields;

        return integration;
    }
}
