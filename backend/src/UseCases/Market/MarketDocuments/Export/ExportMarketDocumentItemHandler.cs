using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Export.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace UseCases.Market.MarketDocuments.Export;

public record ExportMarketDocumentItemCommand();

public class ExportMarketDocumentItemHandler(
    IReadRepository<ServiceStartItem> serviceStartItemReadRepository,
    IReadRepository<Configuration> configurationReadRepository,
    IRepository<MarketDocumentItem> itemRepository,
    IRepository<MarketDocumentReference> referenceRepository,
    MarketDocumentEntityGeneratorResolver generatorResolver,
    IMessageBus bus)
{
    private const string HandlerIdentifier = "handler";

    public async Task<Result<IReadOnlyList<long>>> Handle(
        ExportMarketDocumentItemCommand command, 
        CancellationToken ct)
    {
        var itemIds = new List<long>();

        var configurations = await configurationReadRepository.ListAsync(
            new ConfigurationByTypeSpec(CoreConstants.ConfigurationTypes.Edi), ct);

        foreach (var configuration in configurations)
        {
            var company = configuration.GetRequiredCompany();
            var handlerIdentifier = configuration.GetRequiredValue(HandlerIdentifier);

            var serviceStartItems = await serviceStartItemReadRepository.ListAsync(
                new ServiceStartItemsPendingExportSpec(company.Id), ct);

            foreach (var serviceStartItem in serviceStartItems)
            {
                var (set, subSet) = serviceStartItem.ServiceStart.ServiceStartTypeId switch
                {
                    CoreConstants.ServiceStartTypes.Switch => ("814", "01"),
                    CoreConstants.ServiceStartTypes.MoveIn => ("814", "16"),
                    _ => throw new InvalidOperationException(
                        $"ServiceStart '{serviceStartItem.ServiceStartId}' has an unsupported ServiceStartTypeId '{serviceStartItem.ServiceStart.ServiceStartTypeId}'.")
                };

                var generator = generatorResolver.Resolve($"{handlerIdentifier}_{set}_{subSet}");

                var item = await generator.CreateAsync(serviceStartItem, configuration, ct);
                item.Set = set;
                item.SubSet = subSet;
                item.MarketDocumentId = null;
                item.MarketDocumentItemStatusId = CoreConstants.MarketDocumentItemStatuses.New;

                var created = await itemRepository.AddAsync(item, ct);

                var reference = new MarketDocumentReference
                {
                    MarketDocumentItemId = created.Id,
                    ReferenceTypeId = CoreConstants.MarketDocumentReferenceTypes.ServiceStart,
                    ReferenceId = serviceStartItem.Id,
                };

                await referenceRepository.AddAsync(reference, ct);

                itemIds.Add(created.Id);
            }
        }

        _ = await bus.InvokeAsync<Result<IReadOnlyList<long>>>(new ExportMarketDocumentCommand(), ct);

        return Result.Success<IReadOnlyList<long>>(itemIds);
    }
}
