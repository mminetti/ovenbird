using Ardalis.Result;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Core.Settings;
using Core.Settings.Specifications;
using Microsoft.Extensions.DependencyInjection;
using UseCases.Market.MarketDocuments.Export;
using UseCases.Market.MarketDocuments.Export.Interfaces;
using Wolverine;

namespace UnitTests.UseCases.Market.MarketDocuments;

public class ExportMarketDocumentItemHandlerHandle
{
    private readonly IReadRepository<ServiceStartItem> _serviceStartItemReadRepository =
        Substitute.For<IReadRepository<ServiceStartItem>>();
    private readonly IReadRepository<Configuration> _configurationReadRepository =
        Substitute.For<IReadRepository<Configuration>>();
    private readonly IRepository<MarketDocumentItem> _itemRepository = Substitute.For<IRepository<MarketDocumentItem>>();
    private readonly IRepository<MarketDocumentReference> _referenceRepository =
        Substitute.For<IRepository<MarketDocumentReference>>();
    private readonly IMessageBus _bus = Substitute.For<IMessageBus>();
    private readonly Configuration _configuration = new()
    {
        Id = 1,
        Name = "edi.export",
        Company = new Company { Id = 1, Name = "Acme", TimeZoneId = "UTC" },
        ConfigurationFields =
        [
            new ConfigurationField { ConfigurationId = 1, Name = "handler", Value = "BigData" },
            new ConfigurationField { ConfigurationId = 1, Name = "company.marketer.name", Value = "Acme Energy" },
            new ConfigurationField { ConfigurationId = 1, Name = "company.marketer.identifier", Value = "123456789" },
        ],
    };

    public ExportMarketDocumentItemHandlerHandle()
    {
        _bus
            .InvokeAsync<Result<IReadOnlyList<long>>>(Arg.Any<ExportMarketDocumentCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success<IReadOnlyList<long>>([]));

        _configurationReadRepository
            .ListAsync(Arg.Any<ConfigurationByTypeSpec>(), Arg.Any<CancellationToken>())
            .Returns([_configuration]);
    }

    [Fact]
    public async Task GeneratesAnOrphanMarketDocumentItemAndReferenceForEachPendingServiceStartItem()
    {
        var switchGenerator = Substitute.For<IMarketDocumentEntityGenerator>();
        switchGenerator
            .CreateAsync(Arg.Any<ServiceStartItem>(), Arg.Any<Configuration>(), Arg.Any<CancellationToken>())
            .Returns(new MarketDocumentItem { ReferenceNumber = "SWITCH-1" });

        var moveInGenerator = Substitute.For<IMarketDocumentEntityGenerator>();
        moveInGenerator
            .CreateAsync(Arg.Any<ServiceStartItem>(), Arg.Any<Configuration>(), Arg.Any<CancellationToken>())
            .Returns(new MarketDocumentItem { ReferenceNumber = "MOVEIN-1" });

        var services = new ServiceCollection();
        services.AddKeyedSingleton("BigData_814_01", switchGenerator);
        services.AddKeyedSingleton("BigData_814_16", moveInGenerator);

        var resolver = new MarketDocumentEntityGeneratorResolver(services.BuildServiceProvider());

        var switchServiceStart = new ServiceStart
        {
            Id = 1,
            ServiceStartTypeId = Constants.ServiceStartTypes.Switch,
            ServiceOrderStatusId = Constants.ServiceOrderStatuses.New,
        };
        var switchItem = new ServiceStartItem
        {
            Id = 10,
            ServiceStartId = switchServiceStart.Id,
            ServiceStart = switchServiceStart,
            ServiceActionId = Constants.ServiceActions.Initiate,
            Identifier = "SS-1",
        };

        var moveInServiceStart = new ServiceStart
        {
            Id = 2,
            ServiceStartTypeId = Constants.ServiceStartTypes.MoveIn,
            ServiceOrderStatusId = Constants.ServiceOrderStatuses.New,
        };
        var moveInItem = new ServiceStartItem
        {
            Id = 20,
            ServiceStartId = moveInServiceStart.Id,
            ServiceStart = moveInServiceStart,
            ServiceActionId = Constants.ServiceActions.Initiate,
            Identifier = "SS-2",
        };

        _serviceStartItemReadRepository
            .ListAsync(Arg.Any<ServiceStartItemsPendingExportSpec>(), Arg.Any<CancellationToken>())
            .Returns([switchItem, moveInItem]);

        long nextItemId = 100;
        var createdItems = new List<MarketDocumentItem>();
        _itemRepository
            .AddAsync(Arg.Do<MarketDocumentItem>(i => createdItems.Add(i)), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                var item = callInfo.Arg<MarketDocumentItem>();
                item.Id = nextItemId++;
                return item;
            });

        var createdReferences = new List<MarketDocumentReference>();
        _referenceRepository
            .AddAsync(Arg.Do<MarketDocumentReference>(r => createdReferences.Add(r)), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<MarketDocumentReference>());

        var handler = new ExportMarketDocumentItemHandler(
            _serviceStartItemReadRepository, _configurationReadRepository, _itemRepository, _referenceRepository, resolver, _bus);

        var result = await handler.Handle(new ExportMarketDocumentItemCommand(), CancellationToken.None);

        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe([100, 101]);

        createdItems.Count.ShouldBe(2);

        createdItems[0].Set.ShouldBe("814");
        createdItems[0].SubSet.ShouldBe("01");
        createdItems[0].ReferenceNumber.ShouldBe("SWITCH-1");
        createdItems[0].MarketDocumentId.ShouldBeNull();
        createdItems[0].MarketDocumentItemStatusId.ShouldBe(Constants.MarketDocumentItemStatuses.New);

        createdItems[1].Set.ShouldBe("814");
        createdItems[1].SubSet.ShouldBe("16");
        createdItems[1].ReferenceNumber.ShouldBe("MOVEIN-1");
        createdItems[1].MarketDocumentId.ShouldBeNull();

        createdReferences.Count.ShouldBe(2);
        createdReferences[0].MarketDocumentItemId.ShouldBe(100);
        createdReferences[0].ReferenceTypeId.ShouldBe(Constants.MarketDocumentReferenceTypes.ServiceStart);
        createdReferences[0].ReferenceId.ShouldBe(switchItem.Id);

        createdReferences[1].MarketDocumentItemId.ShouldBe(101);
        createdReferences[1].ReferenceId.ShouldBe(moveInItem.Id);

        await switchGenerator.Received(1).CreateAsync(switchItem, _configuration, Arg.Any<CancellationToken>());
        await moveInGenerator.Received(1).CreateAsync(moveInItem, _configuration, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ThrowsWhenServiceStartTypeIsUnsupported()
    {
        var services = new ServiceCollection();
        var resolver = new MarketDocumentEntityGeneratorResolver(services.BuildServiceProvider());

        var serviceStart = new ServiceStart
        {
            Id = 1,
            ServiceStartTypeId = 999,
            ServiceOrderStatusId = Constants.ServiceOrderStatuses.New,
        };
        var item = new ServiceStartItem
        {
            Id = 10,
            ServiceStartId = serviceStart.Id,
            ServiceStart = serviceStart,
            ServiceActionId = Constants.ServiceActions.Initiate,
        };

        _serviceStartItemReadRepository
            .ListAsync(Arg.Any<ServiceStartItemsPendingExportSpec>(), Arg.Any<CancellationToken>())
            .Returns([item]);

        var handler = new ExportMarketDocumentItemHandler(
            _serviceStartItemReadRepository, _configurationReadRepository, _itemRepository, _referenceRepository, resolver, _bus);

        await Should.ThrowAsync<InvalidOperationException>(
            () => handler.Handle(new ExportMarketDocumentItemCommand(), CancellationToken.None));
    }
}
