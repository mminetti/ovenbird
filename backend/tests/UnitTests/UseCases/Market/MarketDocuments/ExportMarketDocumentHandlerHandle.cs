using Core.MarketExchange;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Export;
using UseCases.Market.MarketDocuments.Export.Interfaces;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class ExportMarketDocumentHandlerHandle
{
    private readonly IRepository<MarketDocument> _documentRepository = Substitute.For<IRepository<MarketDocument>>();
    private readonly IRepository<MarketDocumentItem> _itemRepository = Substitute.For<IRepository<MarketDocumentItem>>();
    private readonly IReadRepository<Configuration> _configurationReadRepository = Substitute.For<IReadRepository<Configuration>>();
    private readonly IEdiExportHandler _ediHandler = Substitute.For<IEdiExportHandler>();
    private readonly ExportMarketDocumentHandler _handler;

    public ExportMarketDocumentHandlerHandle()
    {
        _ediHandler.Identifier.Returns("BigData");

        _handler = new ExportMarketDocumentHandler(
            _documentRepository,
            _itemRepository,
            _configurationReadRepository,
            new EdiExportHandlerResolver([_ediHandler]));
    }

    [Fact]
    public async Task PersistsTheDocumentAndStampsMarketDocumentIdOnEachExportedItem()
    {
        var company = new Company { Id = 1, Name = "Acme", TimeZoneId = "UTC" };
        var configuration = CreateConfiguration(company);

        _configurationReadRepository
            .ListAsync(Arg.Any<ConfigurationByTypeSpec>(), Arg.Any<CancellationToken>())
            .Returns([configuration]);

        var pendingItems = new List<MarketDocumentItem>
        {
            new() { Id = 1, Set = "814", SubSet = "01" },
            new() { Id = 2, Set = "814", SubSet = "16" },
        };

        var document = new MarketDocument { Identifier = "edi.export_20261018.xml", CompanyId = company.Id };

        _ediHandler
            .ExecuteExportAsync(configuration, Arg.Any<CancellationToken>())
            .Returns(new MarketDocumentExportResult(document, pendingItems));

        _documentRepository
            .AddAsync(Arg.Any<MarketDocument>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var doc = callInfo.Arg<MarketDocument>();
                doc.Id = 500;
                return doc;
            });

        var result = await _handler.Handle(new ExportMarketDocumentCommand(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe([500]);

        pendingItems[0].MarketDocumentId.ShouldBe(500L);
        pendingItems[1].MarketDocumentId.ShouldBe(500L);

        await _itemRepository.Received(2).UpdateAsync(Arg.Any<MarketDocumentItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SkipsConfigurationWhenHandlerReturnsNull()
    {
        var configuration = CreateConfiguration(new Company { Id = 1, Name = "Acme", TimeZoneId = "UTC" });

        _configurationReadRepository
            .ListAsync(Arg.Any<ConfigurationByTypeSpec>(), Arg.Any<CancellationToken>())
            .Returns([configuration]);

        _ediHandler
            .ExecuteExportAsync(configuration, Arg.Any<CancellationToken>())
            .Returns((MarketDocumentExportResult?)null);

        var result = await _handler.Handle(new ExportMarketDocumentCommand(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();

        await _documentRepository.DidNotReceive().AddAsync(Arg.Any<MarketDocument>(), Arg.Any<CancellationToken>());
    }

    private static Configuration CreateConfiguration(Company company, string name = "edi.export") =>
        new()
        {
            Id = 1,
            Name = name,
            Company = company,
            CompanyId = company.Id,
            ConfigurationFields =
            [
                new ConfigurationField { ConfigurationId = 1, Name = "handler", Value = "BigData" },
            ],
        };
}
