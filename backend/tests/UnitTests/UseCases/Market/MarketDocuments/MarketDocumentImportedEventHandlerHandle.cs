using Ardalis.Result;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.MarketExchange.Events;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Common;
using UseCases.Market.MarketDocuments.Import;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using Wolverine;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class MarketDocumentImportedEventHandlerHandle
{
    private readonly IRepository<MarketDocumentItem> _itemRepository = Substitute.For<IRepository<MarketDocumentItem>>();
    private readonly IRepository<MarketDocument> _documentRepository = Substitute.For<IRepository<MarketDocument>>();
    private readonly IReadRepository<Configuration> _configurationReadRepository = Substitute.For<IReadRepository<Configuration>>();
    private readonly IEdiImportHandler _ediHandler = Substitute.For<IEdiImportHandler>();
    private readonly IMessageBus _bus = Substitute.For<IMessageBus>();
    private readonly Configuration _configuration;
    private readonly MarketDocumentImportedEventHandler _handler;

    public MarketDocumentImportedEventHandlerHandle()
    {
        _ediHandler.Identifier.Returns("BigData");

        _configuration = CreateConfiguration();

        _configurationReadRepository
            .FirstOrDefaultAsync(Arg.Any<ConfigurationByTypeAndCompanySpec>(), Arg.Any<CancellationToken>())
            .Returns(_configuration);

        _bus
            .InvokeAsync<Result>(Arg.Any<MarketDocumentEntityBuilderCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        _handler = new MarketDocumentImportedEventHandler(
            _itemRepository,
            _documentRepository,
            _configurationReadRepository,
            new EdiImportHandlerResolver([_ediHandler]),
            _bus);
    }

    [Fact]
    public async Task PersistsEachItemReturnedByTheHandler_TriggersEntityBuilding_AndMarksTheDocumentDone()
    {
        var document = new MarketDocument { Id = 1, Identifier = "document.xml", CompanyId = _configuration.CompanyId!.Value };

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

        await _handler.Handle(new MarketDocumentImportedEvent(document), CancellationToken.None);

        created.Count.ShouldBe(2);
        created[0].MarketDocumentId.ShouldBe(document.Id);
        created[1].MarketDocumentId.ShouldBe(document.Id);

        document.StatusId.ShouldBe(Constants.MarketDocumentStatuses.Done);

        await _bus.Received(2).InvokeAsync<Result>(Arg.Any<MarketDocumentEntityBuilderCommand>(), Arg.Any<CancellationToken>());
        await _documentRepository.Received(1).UpdateAsync(document, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task IgnoresAnInvalidEntityBuilderResult_AndStillProcessesRemainingItems()
    {
        var document = new MarketDocument { Id = 1, Identifier = "document.xml", CompanyId = _configuration.CompanyId!.Value };

        var parsedItems = new List<MarketDocumentItem>
        {
            new() { Set = "814", SubSet = "05", ReferenceNumber = "REF-1", ServicePointIdentifier = "ACCT-1" },
            new() { Set = "814", SubSet = "05", ReferenceNumber = "REF-2", ServicePointIdentifier = "ACCT-2" },
        };

        _ediHandler
            .ExecuteItemImportAsync(_configuration, document, Arg.Any<CancellationToken>())
            .Returns(parsedItems);

        long nextId = 500;
        _itemRepository
            .AddAsync(Arg.Any<MarketDocumentItem>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var item = callInfo.Arg<MarketDocumentItem>();
                item.Id = nextId++;
                return item;
            });

        _bus
            .InvokeAsync<Result>(Arg.Any<MarketDocumentEntityBuilderCommand>(), Arg.Any<CancellationToken>())
            .Returns(
                Result.Invalid(new ValidationError(
                    EntityReference.Format(nameof(MarketDocumentItem), 500), "bad data", "CustomerClass", ValidationSeverity.Error)),
                Result.Success());

        await _handler.Handle(new MarketDocumentImportedEvent(document), CancellationToken.None);

        document.StatusId.ShouldBe(Constants.MarketDocumentStatuses.Done);

        await _bus.Received(2).InvokeAsync<Result>(Arg.Any<MarketDocumentEntityBuilderCommand>(), Arg.Any<CancellationToken>());
        await _itemRepository.Received(2).AddAsync(Arg.Any<MarketDocumentItem>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarksTheDocumentAsErrorWhenAnItemFailsToPersist_StoppingRemainingProcessing()
    {
        var document = new MarketDocument { Id = 1, Identifier = "document.xml", CompanyId = _configuration.CompanyId!.Value };

        var parsedItems = new List<MarketDocumentItem>
        {
            new() { Set = "867", SubSet = "03", ReferenceNumber = "REF-1", ServicePointIdentifier = "ACCT-1" },
            new() { Set = "867", SubSet = "03", ReferenceNumber = "REF-2", ServicePointIdentifier = "ACCT-2" },
        };

        _ediHandler
            .ExecuteItemImportAsync(_configuration, document, Arg.Any<CancellationToken>())
            .Returns(parsedItems);

        _itemRepository
            .AddAsync(Arg.Any<MarketDocumentItem>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<MarketDocumentItem>(new InvalidOperationException("boom")));

        await Should.NotThrowAsync(() =>
            _handler.Handle(new MarketDocumentImportedEvent(document), CancellationToken.None));

        document.StatusId.ShouldBe(Constants.MarketDocumentStatuses.Error);

        await _itemRepository.Received(1).AddAsync(Arg.Any<MarketDocumentItem>(), Arg.Any<CancellationToken>());
        await _documentRepository.Received(1).UpdateAsync(document, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarksTheDocumentAsErrorWhenNoEdiConfigurationExistsForTheDocumentsCompany()
    {
        _configurationReadRepository
            .FirstOrDefaultAsync(Arg.Any<ConfigurationByTypeAndCompanySpec>(), Arg.Any<CancellationToken>())
            .Returns((Configuration?)null);

        var document = new MarketDocument { Id = 1, Identifier = "document.xml", CompanyId = 999 };

        await Should.NotThrowAsync(() =>
            _handler.Handle(new MarketDocumentImportedEvent(document), CancellationToken.None));

        document.StatusId.ShouldBe(Constants.MarketDocumentStatuses.Error);

        await _documentRepository.Received(1).UpdateAsync(document, Arg.Any<CancellationToken>());
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
