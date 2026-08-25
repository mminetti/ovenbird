using Core.Shared;
using UseCases.Interfaces.Files;
using UseCases.Market.MarketDocuments;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class DefaultMarketDocumentImportStrategyTests
{
    private readonly ISftpService _sftpService = Substitute.For<ISftpService>();
    private readonly BigDataConnectionStrategy _strategy;

    public DefaultMarketDocumentImportStrategyTests()
    {
        _strategy = new BigDataConnectionStrategy(_sftpService);
    }

    [Fact]
    public async Task ListFilesAsyncDelegatesToSftpService()
    {
        _sftpService.ListAsync("remote/dir", Arg.Any<CancellationToken>())
            .Returns(["remote/dir/file1.csv"]);

        var market = new Core.Market.Market { Id = 1, Identifier = "default", TimeZoneId = "UTC" };
        var company = new Company { Id = 1, Name = "Acme", MarketId = 1, Market = market };

        var files = await _strategy.ListFilesAsync(company, "remote/dir", CancellationToken.None);

        files.ShouldBe(["remote/dir/file1.csv"]);
    }
}
