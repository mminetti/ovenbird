using Core.Market;
using Core.Service;
using Core.Service.Specifications;
using UseCases.Market.MarketDocuments.Export.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace UseCases.Market.MarketDocuments.Export;

public class ExportMarketDocumentItemHandler(
    IReadRepository<ServiceStartItem> serviceStartItemReadRepository,
    IRepository<MarketDocumentItem> itemRepository,
    IRepository<MarketDocumentReference> referenceRepository,
    MarketDocumentEntityGeneratorResolver generatorResolver)
{
    public async Task<Result<IReadOnlyList<long>>> Handle(ExportMarketDocumentItemCommand command, CancellationToken ct)
    {
        var itemIds = new List<long>();

        var serviceStartItems = await serviceStartItemReadRepository.ListAsync(
            new ServiceStartItemsPendingExportSpec(), ct);

        foreach (var serviceStartItem in serviceStartItems)
        {
            var (set, subSet) = serviceStartItem.ServiceStart.ServiceStartTypeId switch
            {
                CoreConstants.ServiceStartTypes.Switch => ("814", "01"),
                CoreConstants.ServiceStartTypes.MoveIn => ("814", "16"),
                _ => throw new InvalidOperationException(
                    $"ServiceStart '{serviceStartItem.ServiceStartId}' has an unsupported ServiceStartTypeId '{serviceStartItem.ServiceStart.ServiceStartTypeId}'.")
            };

            var generator = generatorResolver.Resolve($"{set}_{subSet}");

            var item = await generator.CreateAsync(serviceStartItem, ct);
            item.Set = set;
            item.SubSet = subSet;
            item.MarketDocumentId = null;
            item.MarketDocumentItemStatusId = CoreConstants.MarketDocumentItemStatuses.New;

            var created = await itemRepository.AddAsync(item, ct);

            await referenceRepository.AddAsync(new MarketDocumentReference
            {
                MarketDocumentItemId = created.Id,
                ReferenceTypeId = CoreConstants.MarketDocumentReferenceTypes.ServiceStart,
                ReferenceId = serviceStartItem.Id,
            }, ct);

            itemIds.Add(created.Id);
        }

        return Result.Success<IReadOnlyList<long>>(itemIds);
    }
}
