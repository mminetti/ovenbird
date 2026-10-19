using Ardalis.Result;
using Core.MarketExchange;
using Core.MarketExchange.Specifications;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Import;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using Wolverine;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class ImportMarketDocumentItemHandlerHandle
{
    private readonly IRepository<MarketDocumentItem> _itemRepository = Substitute.For<IRepository<MarketDocumentItem>>();
    private readonly IReadRepository<MarketDocument> _documentReadRepository = Substitute.For<IReadRepository<MarketDocument>>();
    private readonly IReadRepository<Configuration> _configurationReadRepository = Substitute.For<IReadRepository<Configuration>>();
    private readonly IEdiImportHandler _ediHandler = Substitute.For<IEdiImportHandler>();
    private readonly IMessageBus _bus = Substitute.For<IMessageBus>();
    private readonly Configuration _configuration;
    private readonly ImportMarketDocumentItemHandler _handler;

    public ImportMarketDocumentItemHandlerHandle()
    {
        _ediHandler.Identifier.Returns("BigData");

        _configuration = CreateConfiguration();

        _configurationReadRepository
            .FirstOrDefaultAsync(Arg.Any<ConfigurationByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(_configuration);

        _handler = new ImportMarketDocumentItemHandler(
            _itemRepository,
            _documentReadRepository,
            _configurationReadRepository,
            new EdiImportHandlerResolver([_ediHandler]),
            _bus);
    }

    [Fact]
    public async Task PersistsEachItemReturnedByTheHandler_AndTriggersEntityBuilding()
    {
        var document = new MarketDocument { Id = 1, Identifier = "document.xml", CompanyId = _configuration.CompanyId!.Value };

        _documentReadRepository
            .FirstOrDefaultAsync(Arg.Any<MarketDocumentByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(document);

        var parsedItems = new List<MarketDocumentItem>
        {
            new() { Set = "867", SubSet = "03", ReferenceNumber = "REF-1", ServicePointIdentifier = "ACCT-1" },
            new() { Set = "810", SubSet = "02", ReferenceNumber = "BILL-1", ServicePointIdentifier = "LDC-1" },
        };

        _ediHandler
            .ExecuteItemImportAsync(_configuration, document, Arg.Any<CancellationToken>())
            .Returns(parsedItems);

        long nextId = 500;
        var created = new List<MarketDocumentItem>();
        _itemRepository
            .AddAsync(Arg.Do<MarketDocumentItem>(i => created.Add(i)), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var item = callInfo.Arg<MarketDocumentItem>();
                item.Id = nextId++;
                return item;
            });

        var result = await _handler.Handle(
            new ImportMarketDocumentItemCommand(_configuration.Id, document.Id), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe([500, 501]);

        created.Count.ShouldBe(2);
        created[0].MarketDocumentId.ShouldBe(document.Id);
        created[1].MarketDocumentId.ShouldBe(document.Id);

        await _bus.Received(2).InvokeAsync(Arg.Any<MarketDocumentEntityBuilderCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ThrowsWhenMarketDocumentIsNotFound()
    {
        _documentReadRepository
            .FirstOrDefaultAsync(Arg.Any<MarketDocumentByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((MarketDocument?)null);

        await Should.ThrowAsync<InvalidOperationException>(() => _handler.Handle(
            new ImportMarketDocumentItemCommand(_configuration.Id, 1), CancellationToken.None));
    }

    private static Configuration CreateConfiguration()
    {
        var company = new Company { Id = 1, Name = "Acme", TimeZoneId = "UTC" };

        return new Configuration
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
}
