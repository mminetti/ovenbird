using Ardalis.Result;
using Core.MarketExchange;
using Core.Settings;
using Core.Settings.Specifications;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Common;
using UseCases.Market.MarketDocuments.Import;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class MarketDocumentEntityBuilderHandlerHandle
{
    private const string Key = "BigData_867_02";

    private readonly IReadRepository<MarketDocumentItem> _itemReadRepository = Substitute.For<IReadRepository<MarketDocumentItem>>();
    private readonly IReadRepository<Configuration> _configurationReadRepository = Substitute.For<IReadRepository<Configuration>>();

    private readonly Configuration _configuration = new()
    {
        Id = 1,
        Name = "edi.import",
        ConfigurationFields = [new ConfigurationField { ConfigurationId = 1, Name = "handler", Value = "BigData" }],
    };

    public MarketDocumentEntityBuilderHandlerHandle()
    {
        _configurationReadRepository
            .FirstOrDefaultAsync(Arg.Any<ConfigurationByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns(_configuration);
    }

    [Fact]
    public async Task CreatesTheEntityWhenABuilderIsRegisteredForTheItemsSetAndSubSet()
    {
        var item = new MarketDocumentItem { Id = 1, Set = "867", SubSet = "02" };

        _itemReadRepository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);

        var builder = Substitute.For<IMarketDocumentEntityBuilder>();
        builder.CreateAsync(item, Arg.Any<CancellationToken>()).Returns(Result.Success());

        var services = new ServiceCollection();
        services.AddKeyedSingleton(Key, builder);

        var handler = new MarketDocumentEntityBuilderHandler(
            _itemReadRepository, _configurationReadRepository, new MarketDocumentEntityBuilderResolver(services.BuildServiceProvider()));

        var result = await handler.Handle(new MarketDocumentEntityBuilderCommand(_configuration.Id, item.Id), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        await builder.Received(1).CreateAsync(item, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PromotesTheBuilderFieldIdentifierIntoErrorCode_AndTagsTheItemAsTheEntityReference()
    {
        var item = new MarketDocumentItem { Id = 1, Set = "867", SubSet = "02" };

        _itemReadRepository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);

        var builder = Substitute.For<IMarketDocumentEntityBuilder>();
        builder.CreateAsync(item, Arg.Any<CancellationToken>())
            .Returns(Result.Invalid(new ValidationError("CustomerClass", "Unmapped customer class 'Residential'.")));

        var services = new ServiceCollection();
        services.AddKeyedSingleton(Key, builder);

        var handler = new MarketDocumentEntityBuilderHandler(
            _itemReadRepository, _configurationReadRepository, new MarketDocumentEntityBuilderResolver(services.BuildServiceProvider()));

        var result = await handler.Handle(new MarketDocumentEntityBuilderCommand(_configuration.Id, item.Id), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e =>
            e.Identifier == EntityReference.Format(nameof(MarketDocumentItem), item.Id)
            && e.ErrorCode == "CustomerClass"
            && e.ErrorMessage == "Unmapped customer class 'Residential'.");
    }

    [Fact]
    public async Task DoesNothingWhenNoBuilderIsRegisteredForTheItemsSetAndSubSet()
    {
        var item = new MarketDocumentItem { Id = 1, Set = "810", SubSet = "02" };

        _itemReadRepository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);

        var services = new ServiceCollection();

        var handler = new MarketDocumentEntityBuilderHandler(
            _itemReadRepository, _configurationReadRepository, new MarketDocumentEntityBuilderResolver(services.BuildServiceProvider()));

        var result = await handler.Handle(new MarketDocumentEntityBuilderCommand(_configuration.Id, item.Id), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task ReturnsInvalidTaggedWithTheItemEntityReferenceWhenBuilderThrows()
    {
        var item = new MarketDocumentItem { Id = 1, Set = "867", SubSet = "02", ReferenceNumber = "REF-1" };

        _itemReadRepository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);

        var builder = Substitute.For<IMarketDocumentEntityBuilder>();
        builder.CreateAsync(item, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<Result>(new InvalidOperationException("boom")));

        var services = new ServiceCollection();
        services.AddKeyedSingleton(Key, builder);

        var handler = new MarketDocumentEntityBuilderHandler(
            _itemReadRepository, _configurationReadRepository, new MarketDocumentEntityBuilderResolver(services.BuildServiceProvider()));

        var result = await handler.Handle(new MarketDocumentEntityBuilderCommand(_configuration.Id, item.Id), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.Invalid);
        result.ValidationErrors.ShouldContain(e =>
            e.Identifier == EntityReference.Format(nameof(MarketDocumentItem), item.Id)
            && e.ErrorCode == string.Empty
            && e.ErrorMessage == "boom");
    }

    [Fact]
    public async Task ReturnsNotFoundWhenMarketDocumentItemCannotBeFound()
    {
        _itemReadRepository.GetByIdAsync(1, Arg.Any<CancellationToken>()).Returns((MarketDocumentItem?)null);

        var services = new ServiceCollection();

        var handler = new MarketDocumentEntityBuilderHandler(
            _itemReadRepository, _configurationReadRepository, new MarketDocumentEntityBuilderResolver(services.BuildServiceProvider()));

        var result = await handler.Handle(new MarketDocumentEntityBuilderCommand(_configuration.Id, 1), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }

    [Fact]
    public async Task ReturnsNotFoundWhenConfigurationCannotBeFound()
    {
        var item = new MarketDocumentItem { Id = 1, Set = "867", SubSet = "02" };

        _itemReadRepository.GetByIdAsync(item.Id, Arg.Any<CancellationToken>()).Returns(item);

        _configurationReadRepository
            .FirstOrDefaultAsync(Arg.Any<ConfigurationByIdSpec>(), Arg.Any<CancellationToken>())
            .Returns((Configuration?)null);

        var services = new ServiceCollection();

        var handler = new MarketDocumentEntityBuilderHandler(
            _itemReadRepository, _configurationReadRepository, new MarketDocumentEntityBuilderResolver(services.BuildServiceProvider()));

        var result = await handler.Handle(new MarketDocumentEntityBuilderCommand(_configuration.Id, item.Id), CancellationToken.None);

        result.Status.ShouldBe(ResultStatus.NotFound);
    }
}
