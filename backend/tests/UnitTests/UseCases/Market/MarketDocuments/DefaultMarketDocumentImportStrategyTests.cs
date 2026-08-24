using Core.Shared;
using UseCases.Interfaces.Files;
using UseCases.Market.MarketDocuments;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class DefaultMarketDocumentImportStrategyTests
{
    private readonly IFtpService _ftpService = Substitute.For<IFtpService>();
    private readonly IFileStorage _fileStorage = Substitute.For<IFileStorage>();
    private readonly TimeProvider _timeProvider = Substitute.For<TimeProvider>();
    private readonly BigDataMarketDocumentStrategy _strategy;

    public DefaultMarketDocumentImportStrategyTests()
    {
        _strategy = new BigDataMarketDocumentStrategy(_ftpService);
    }

    [Fact]
    public async Task ListFilesAsyncDelegatesToFtpService()
    {
        _ftpService.ListFilesAsync("remote/dir", Arg.Any<CancellationToken>())
            .Returns((IReadOnlyList<string>)["remote/dir/file1.csv"]);

        var market = new Core.Market.Market { Id = 1, Identifier = "default", TimeZoneId = "UTC" };
        var company = new Company { Id = 1, Name = "Acme", MarketId = 1, Market = market };

        var files = await _strategy.ListFilesAsync(company, "remote/dir", CancellationToken.None);

        files.ShouldBe(["remote/dir/file1.csv"]);
    }
}
