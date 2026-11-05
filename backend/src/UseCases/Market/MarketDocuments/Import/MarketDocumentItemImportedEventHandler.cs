using Core.Common;
using Core.MarketExchange;
using Core.MarketExchange.Events;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UseCases.Market.MarketDocuments.Import;

public class MarketDocumentItemImportedEventHandler(
    IReadRepository<Configuration> configurationReadRepository,
    IRepository<MarketDocumentItem> itemRepository,
    IRepository<MarketDocumentEvent> eventRepository,
    IRepository<Case> caseRepository,
    MarketDocumentEntityBuilderResolver entityBuilderResolver)
{
    private const string HandlerIdentifier = "handler";

    public async Task Handle(MarketDocumentItemImportedEvent domainEvent, CancellationToken ct)
    {
        var item = domainEvent.MarketDocumentItem;
        Case? @case = null;

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

                if (result.IsSuccess)
                {
                    foreach (var referenceId in result.Value)
                    {
                        await eventRepository.AddAsync(new MarketDocumentEvent
                        {
                            MarketDocumentItemId = item.Id,
                            EventTypeId = entityBuilder.EventTypeId,
                            ReferenceId = referenceId,
                            StatusId = Constants.MarketDocumentEventStatuses.Done,
                        }, ct);
                    }

                    item.MarketDocumentItemStatusId = Constants.MarketDocumentItemStatuses.Done;
                }
                else
                {
                    @case = CaseFactory.Create(Constants.CaseEntityTypes.MarketDocumentItem, item.Id, result.ValidationErrors);
                    item.MarketDocumentItemStatusId = Constants.MarketDocumentItemStatuses.Error;
                }
            }
            else
            {
                item.MarketDocumentItemStatusId = Constants.MarketDocumentItemStatuses.Done;
            }
        }
        catch (Exception ex)
        {
            @case = CaseFactory.Create(Constants.CaseEntityTypes.MarketDocumentItem, item.Id, ex);
            item.MarketDocumentItemStatusId = Constants.MarketDocumentItemStatuses.Error;
        }

        await itemRepository.UpdateAsync(item, ct);

        if (@case is not null)
        {
            await caseRepository.AddAsync(@case, ct);
        }
    }
}
