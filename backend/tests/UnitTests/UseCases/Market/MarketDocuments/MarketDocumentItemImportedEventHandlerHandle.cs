using Ardalis.Result;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.MarketExchange.Events;
using Core.Settings;
using Core.Settings.Specifications;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Market.MarketDocuments.Import;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class MarketDocumentItemImportedEventHandlerHandle
{
    private const string Key = "BigData_867_02";

    private readonly IReadRepository<Configuration> _configurationReadRepository = Substitute.For<IReadRepository<Configuration>>();
    private readonly IRepository<MarketDocumentItem> _itemRepository = Substitute.For<IRepository<MarketDocumentItem>>();
    private readonly IRepository<MarketDocumentEvent> _eventRepository = Substitute.For<IRepository<MarketDocumentEvent>>();

    private readonly Company _company = new() { Id = 1, Name = "Acme", TimeZoneId = "UTC" };

    private readonly Configuration _configuration = new()
    {
        Id = 1,
        Name = "edi.import",
        CompanyId = 1,
        ConfigurationFields = [new ConfigurationField { ConfigurationId = 1, Name = "handler", Value = "BigData" }],
    };

    public MarketDocumentItemImportedEventHandlerHandle()
    {
        _configurationReadRepository
            .FirstOrDefaultAsync(Arg.Any<ConfigurationByTypeAndCompanySpec>(), Arg.Any<CancellationToken>())
            .Returns(_configuration);
    }

    private MarketDocumentItem CreateItem(string set, string subSet) =>
        new()
        {
            Id = 1,
            Set = set,
            SubSet = subSet,
            MarketDocument = new MarketDocument { Id = 1, CompanyId = _company.Id },
        };

    private MarketDocumentItemImportedEventHandler CreateHandler(IServiceProvider services) =>
        new(_configurationReadRepository, _itemRepository, _eventRepository, new MarketDocumentEntityBuilderResolver(services));

    [Fact]
    public async Task CreatesTheEntityWhenABuilderIsRegisteredForTheItemsSetAndSubSet_AndMarksTheItemDone()
    {
        var item = CreateItem("867", "02");

        var builder = Substitute.For<IMarketDocumentEntityBuilder>();
        builder.EventTypeId.Returns(Constants.MarketDocumentEventTypes.HistoricalUsage);
        builder.CreateAsync(item, Arg.Any<CancellationToken>()).Returns(Result.Success<IReadOnlyList<long>>([42]));

        var services = new ServiceCollection();
        services.AddKeyedSingleton(Key, builder);

        var handler = CreateHandler(services.BuildServiceProvider());

        await handler.Handle(new MarketDocumentItemImportedEvent(item), CancellationToken.None);

        await builder.Received(1).CreateAsync(item, Arg.Any<CancellationToken>());

        item.MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.Done);
        await _itemRepository.Received(1).UpdateAsync(item, Arg.Any<CancellationToken>());

        await _eventRepository.Received(1).AddAsync(
            Arg.Is<MarketDocumentEvent>(e =>
                e.MarketDocumentItemId == item.Id &&
                e.EventTypeId == Constants.MarketDocumentEventTypes.HistoricalUsage &&
                e.ReferenceId == 42 &&
                e.StatusId == Constants.MarketDocumentEventStatuses.Done),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RecordsAnErrorEventWithOneItemPerValidationError_WhenTheBuilderReturnsInvalid()
    {
        var item = CreateItem("867", "02");

        var builder = Substitute.For<IMarketDocumentEntityBuilder>();
        builder.EventTypeId.Returns(Constants.MarketDocumentEventTypes.HistoricalUsage);
        builder.CreateAsync(item, Arg.Any<CancellationToken>()).Returns(
            Result<IReadOnlyList<long>>.Invalid(new List<ValidationError>
            {
                new("Field1", "Field1 is required.", string.Empty, ValidationSeverity.Error),
                new("Field2", "Field2 is out of range.", string.Empty, ValidationSeverity.Warning),
            }));

        var services = new ServiceCollection();
        services.AddKeyedSingleton(Key, builder);

        var handler = CreateHandler(services.BuildServiceProvider());

        await handler.Handle(new MarketDocumentItemImportedEvent(item), CancellationToken.None);

        item.MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.Error);

        await _eventRepository.Received(1).AddAsync(
            Arg.Is<MarketDocumentEvent>(e =>
                e.MarketDocumentItemId == item.Id &&
                e.EventTypeId == Constants.MarketDocumentEventTypes.HistoricalUsage &&
                e.ReferenceId == null &&
                e.StatusId == Constants.MarketDocumentEventStatuses.Error &&
                e.Items.Count == 2 &&
                e.Items.Any(i => i.Field == "Field1" && i.Message == "Field1 is required." && i.SeverityId == Constants.ErrorSeverities.Error) &&
                e.Items.Any(i => i.Field == "Field2" && i.Message == "Field2 is out of range." && i.SeverityId == Constants.ErrorSeverities.Warning)),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DoesNothingButStillMarksTheItemDone_WhenNoBuilderIsRegisteredForTheItemsSetAndSubSet()
    {
        var item = CreateItem("810", "02");

        var services = new ServiceCollection();

        var handler = CreateHandler(services.BuildServiceProvider());

        await Should.NotThrowAsync(() =>
            handler.Handle(new MarketDocumentItemImportedEvent(item), CancellationToken.None));

        item.MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.Done);
        await _itemRepository.Received(1).UpdateAsync(item, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarksTheItemAsErrorWhenBuilderThrows()
    {
        var item = CreateItem("867", "02");
        item.ReferenceNumber = "REF-1";

        var builder = Substitute.For<IMarketDocumentEntityBuilder>();
        builder.CreateAsync(item, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Result<IReadOnlyList<long>>>(new InvalidOperationException("boom")));

        var services = new ServiceCollection();
        services.AddKeyedSingleton(Key, builder);

        var handler = CreateHandler(services.BuildServiceProvider());

        await Should.NotThrowAsync(() =>
            handler.Handle(new MarketDocumentItemImportedEvent(item), CancellationToken.None));

        await builder.Received(1).CreateAsync(item, Arg.Any<CancellationToken>());

        item.MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.Error);
        await _itemRepository.Received(1).UpdateAsync(item, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarksTheItemAsErrorWhenItHasNoAssociatedMarketDocument()
    {
        var item = new MarketDocumentItem { Id = 1, Set = "867", SubSet = "02" };

        var services = new ServiceCollection();

        var handler = CreateHandler(services.BuildServiceProvider());

        await Should.NotThrowAsync(() =>
            handler.Handle(new MarketDocumentItemImportedEvent(item), CancellationToken.None));

        item.MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.Error);
        await _itemRepository.Received(1).UpdateAsync(item, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MarksTheItemAsErrorWhenConfigurationCannotBeFound()
    {
        var item = CreateItem("867", "02");

        _configurationReadRepository
            .FirstOrDefaultAsync(Arg.Any<ConfigurationByTypeAndCompanySpec>(), Arg.Any<CancellationToken>())
            .Returns((Configuration?)null);

        var services = new ServiceCollection();

        var handler = CreateHandler(services.BuildServiceProvider());

        await Should.NotThrowAsync(() =>
            handler.Handle(new MarketDocumentItemImportedEvent(item), CancellationToken.None));

        item.MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.Error);
        await _itemRepository.Received(1).UpdateAsync(item, Arg.Any<CancellationToken>());
    }
}
