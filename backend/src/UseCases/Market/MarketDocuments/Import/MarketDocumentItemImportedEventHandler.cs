using Core.MarketExchange;
using Core.MarketExchange.Events;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UseCases.Market.MarketDocuments.Import;

public class MarketDocumentItemImportedEventHandler(
    IReadRepository<Configuration> configurationReadRepository,
    IRepository<MarketDocumentItem> itemRepository,
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
                // we are missing a concept here: MarketDocumentEvent - lets discuss
                // MarketDocumentEventItem will have the list of values to fix on the import

                await entityBuilder.CreateAsync(item, ct);
            }

            item.MarketDocumentItemStatusId = Constants.MarketDocumentItemStatuses.Done;
        }
        catch (Exception ex)
        {
            item.MarketDocumentItemStatusId = Constants.MarketDocumentItemStatuses.Error;
            item.Message = ex.Message;
        }

        await itemRepository.UpdateAsync(item, ct);
    }
}
