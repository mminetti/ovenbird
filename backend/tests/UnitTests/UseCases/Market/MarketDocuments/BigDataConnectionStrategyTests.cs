using Core.Shared;
using UseCases.Interfaces.Files;
using UseCases.Market.MarketDocuments;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class BigDataConnectionStrategyTests
{
    private readonly ISftpService _sftpService = Substitute.For<ISftpService>();
    private readonly BigDataConnectionStrategy _strategy;

    public BigDataConnectionStrategyTests()
    {
        _strategy = new BigDataConnectionStrategy(_sftpService);
    }

    [Fact]
    public async Task ListFilesAsyncDelegatesToSftpServiceUsingCompanySftpIntegration()
    {
        var company = CreateCompanyWithSftpIntegration("sftp.example.com", "2222", "user", "pass");

        _sftpService.ListAsync(
                Arg.Is<SftpOptions>(o =>
                    o.Host == "sftp.example.com" && o.Port == 2222 && o.Username == "user" && o.Password == "pass"),
                "remote/dir",
                Arg.Any<CancellationToken>())
            .Returns(["remote/dir/file1.csv"]);

        var files = await _strategy.ListFilesAsync(company, "remote/dir", CancellationToken.None);

        files.ShouldBe(["remote/dir/file1.csv"]);
    }

    [Fact]
    public async Task ListFilesAsyncDefaultsPortWhenFieldMissing()
    {
        var company = CreateCompanyWithSftpIntegration("sftp.example.com", port: null, "user", "pass");

        _sftpService.ListAsync(
                Arg.Is<SftpOptions>(o => o.Port == 22),
                "remote/dir",
                Arg.Any<CancellationToken>())
            .Returns([]);

        await _strategy.ListFilesAsync(company, "remote/dir", CancellationToken.None);

        await _sftpService.Received(1).ListAsync(Arg.Is<SftpOptions>(o => o.Port == 22), "remote/dir", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ListFilesAsyncThrowsWhenSftpIntegrationIsMissing()
    {
        var market = new Core.Market.Market { Id = 1, Identifier = "default", TimeZoneId = "UTC" };
        var company = new Company { Id = 1, Name = "Acme", MarketId = 1, Market = market };

        await Should.ThrowAsync<InvalidOperationException>(
            () => _strategy.ListFilesAsync(company, "remote/dir", CancellationToken.None));
    }

    [Fact]
    public async Task ListFilesAsyncThrowsWhenRequiredFieldIsMissing()
    {
        var company = CreateCompanyWithSftpIntegration(host: null, "22", "user", "pass");

        await Should.ThrowAsync<InvalidOperationException>(
            () => _strategy.ListFilesAsync(company, "remote/dir", CancellationToken.None));
    }

    private static Company CreateCompanyWithSftpIntegration(string? host, string? port, string? username, string? password)
    {
        var market = new Core.Market.Market { Id = 1, Identifier = "default", TimeZoneId = "UTC" };
        var company = new Company { Id = 1, Name = "Acme", MarketId = 1, Market = market };

        var integration = new SystemIntegration { Id = 1, Identifier = "Sftp", Name = "Sftp", CompanyId = company.Id };

        var fields = new List<SystemIntegrationField>();

        if (host is not null)
        {
            fields.Add(new SystemIntegrationField { SystemIntegrationId = integration.Id, Identifier = "Host", Name = "Host", Value = host });
        }

        if (port is not null)
        {
            fields.Add(new SystemIntegrationField { SystemIntegrationId = integration.Id, Identifier = "Port", Name = "Port", Value = port });
        }

        if (username is not null)
        {
            fields.Add(new SystemIntegrationField { SystemIntegrationId = integration.Id, Identifier = "Username", Name = "Username", Value = username });
        }

        if (password is not null)
        {
            fields.Add(new SystemIntegrationField { SystemIntegrationId = integration.Id, Identifier = "Password", Name = "Password", Value = password });
        }

        integration.SystemIntegrationFields = fields;
        company.SystemIntegrations = [integration];

        return company;
    }
}
