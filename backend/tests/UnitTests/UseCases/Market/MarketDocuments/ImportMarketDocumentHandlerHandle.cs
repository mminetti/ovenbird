using Ardalis.Result;
using Core.MarketExchange;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Import;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using Wolverine;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class ImportMarketDocumentHandlerHandle
{
    private readonly IRepository<MarketDocument> _documentRepository = Substitute.For<IRepository<MarketDocument>>();
    private readonly IReadRepository<Configuration> _configurationReadRepository = Substitute.For<IReadRepository<Configuration>>();
    private readonly IEdiImportHandler _ediHandler = Substitute.For<IEdiImportHandler>();
    private readonly IMessageBus _bus = Substitute.For<IMessageBus>();
    private readonly ImportMarketDocumentHandler _handler;

    public ImportMarketDocumentHandlerHandle()
    {
        _ediHandler.Identifier.Returns("BigData");

        _bus
            .InvokeAsync<Result<IReadOnlyList<long>>>(Arg.Any<ImportMarketDocumentItemCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<IReadOnlyList<long>>([]));

        _handler = new ImportMarketDocumentHandler(
            _documentRepository,
            _configurationReadRepository,
            new EdiImportHandlerResolver([_ediHandler]),
            _bus);
    }

    [Fact]
    public async Task PersistsNewDocumentsAndTriggersItemImport_ButSkipsAlreadyExistingDocuments()
    {
        var company = new Company { Id = 1, Name = "Acme", TimeZoneId = "UTC" };
        var configuration = CreateConfiguration(company);

        _configurationReadRepository
            .ListAsync(Arg.Any<ConfigurationByTypeSpec>(), Arg.Any<CancellationToken>())
            .Returns([configuration]);

        var existingDocument = new MarketDocument { Id = 99, Identifier = "existingfile.csv", CompanyId = company.Id };
        var newDocument = new MarketDocument { Identifier = "newfile.csv", CompanyId = company.Id };

        _ediHandler
            .ExecuteImportAsync(configuration, Arg.Any<CancellationToken>())
            .Returns(new List<MarketDocument> { existingDocument, newDocument });

        long nextId = 100;
        MarketDocument? created = null;
        _documentRepository
            .AddAsync(Arg.Do<MarketDocument>(d => created = d), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var document = callInfo.Arg<MarketDocument>();
                document.Id = nextId++;
                return document;
            });

        var result = await _handler.Handle(new ImportMarketDocumentCommand(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe([existingDocument.Id, 100]);

        created.ShouldBeSameAs(newDocument);

        await _documentRepository.Received(1).AddAsync(Arg.Any<MarketDocument>(), Arg.Any<CancellationToken>());
        await _bus.Received(1).InvokeAsync<Result<IReadOnlyList<long>>>(
            Arg.Is<ImportMarketDocumentItemCommand>(c => c.ConfigurationId == configuration.Id && c.MarketDocumentId == 100),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SkipsConfigurationWhenHandlerReturnsNoDocuments()
    {
        var configuration = CreateConfiguration(new Company { Id = 1, Name = "Acme", TimeZoneId = "UTC" });

        _configurationReadRepository
            .ListAsync(Arg.Any<ConfigurationByTypeSpec>(), Arg.Any<CancellationToken>())
            .Returns([configuration]);

        _ediHandler
            .ExecuteImportAsync(configuration, Arg.Any<CancellationToken>())
            .Returns(new List<MarketDocument>());

        var result = await _handler.Handle(new ImportMarketDocumentCommand(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBeEmpty();

        await _documentRepository.DidNotReceive().AddAsync(Arg.Any<MarketDocument>(), Arg.Any<CancellationToken>());
    }

    private static Configuration CreateConfiguration(Company company) =>
        new()
        {
            Id = 1,
            Name = "edi.import",
            Company = company,
            CompanyId = company.Id,
            ConfigurationFields =
            [
                new ConfigurationField { ConfigurationId = 1, Name = "handler", Value = "BigData" },
            ],
        };
}
