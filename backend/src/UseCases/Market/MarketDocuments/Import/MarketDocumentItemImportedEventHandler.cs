using Core.MarketExchange;
using Core.MarketExchange.Events;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Common;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UseCases.Market.MarketDocuments.Import;

public class MarketDocumentItemImportedEventHandler(
    IReadRepository<Configuration> configurationReadRepository,
    IRepository<MarketDocumentItem> itemRepository,
    IRepository<MarketDocumentEvent> eventRepository,
    MarketDocumentEntityBuilderResolver entityBuilderResolver)
{
    private const string HandlerIdentifier = "handler";

    public async Task Handle(MarketDocumentItemImportedEvent domainEvent, CancellationToken ct)
    {
        var item = domainEvent.MarketDocumentItem;

        try
        {
            var companyId = item.MarketDocument?.CompanyId
                ?? throw new InvalidOperationException($"MarketDocumentItem '{item.Id}' has no associated MarketDocument.");

            var configuration = await configurationReadRepository.FirstOrDefaultAsync(
                new ConfigurationByTypeAndCompanySpec(Constants.ConfigurationTypes.Edi, companyId), ct)
                ?? throw new InvalidOperationException($"No EDI configuration was found for company '{companyId}'.");

            var handlerIdentifier = configuration.GetRequiredValue(HandlerIdentifier);
            var entityBuilder = entityBuilderResolver.Resolve($"{handlerIdentifier}_{item.Set}_{item.SubSet}");

            if (entityBuilder is not null)
            {
                var result = await entityBuilder.CreateAsync(item, ct);

                var marketDocumentEvents = ToMarketDocumentEvents(item.Id, entityBuilder.EventTypeId, result);

                foreach (var marketDocumentEvent in marketDocumentEvents)
                {
                    await eventRepository.AddAsync(marketDocumentEvent, ct);
                }

                item.MarketDocumentItemStatusId = result.IsSuccess
                    ? Constants.MarketDocumentItemStatuses.Done
                    : Constants.MarketDocumentItemStatuses.Error;
            }
            else
            {
                item.MarketDocumentItemStatusId = Constants.MarketDocumentItemStatuses.Done;
            }
        }
        catch (Exception ex)
        {
            item.MarketDocumentItemStatusId = Constants.MarketDocumentItemStatuses.Error;
            item.Message = ex.Message;
        }

        await itemRepository.UpdateAsync(item, ct);
    }

    private static IEnumerable<MarketDocumentEvent> ToMarketDocumentEvents(
        long itemId, 
        int eventTypeId, 
        Result<IReadOnlyList<long>> result)
    {
        if (result.IsSuccess)
        {
            return result.Value.Select(referenceId => new MarketDocumentEvent
            {
                MarketDocumentItemId = itemId,
                EventTypeId = eventTypeId,
                ReferenceId = referenceId,
                StatusId = Constants.MarketDocumentEventStatuses.Done,
            });
        }

        return
        [
            new MarketDocumentEvent
            {
                MarketDocumentItemId = itemId,
                EventTypeId = eventTypeId,
                StatusId = Constants.MarketDocumentEventStatuses.Error,
                Items = [.. result.ValidationErrors.Select(ErrorLogItemFactory.Create<MarketDocumentEventItem>)],
            },
        ];
    }
}
