using Core.MarketExchange;
using Core.MarketExchange.Events;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace UseCases.Market.MarketDocuments.Import;

public class MarketDocumentImportedEventHandler(
    IRepository<MarketDocumentItem> itemRepository,
    IRepository<MarketDocument> documentRepository,
    IReadRepository<Configuration> configurationRepository,
    EdiImportHandlerResolver ediResolver,
    IMessageBus bus)
{
    private const string HandlerIdentifier = "handler";

    public async Task Handle(MarketDocumentImportedEvent domainEvent, CancellationToken ct)
    {
        var document = domainEvent.MarketDocument;

        try
        {
            var configuration = await configurationRepository.FirstOrDefaultAsync(
                new ConfigurationByTypeAndCompanySpec(CoreConstants.ConfigurationTypes.Edi, document.CompanyId), ct)
                ?? throw new InvalidOperationException($"No EDI configuration was found for company '{document.CompanyId}'.");

            var ediHandler = ediResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));

            var items = await ediHandler.ExecuteItemImportAsync(configuration, document, ct);

            foreach (var item in items)
            {
                item.MarketDocumentId = document.Id;

                var created = await itemRepository.AddAsync(item, ct);

                await bus.InvokeAsync<Result>(
                    new MarketDocumentEntityBuilderCommand(configuration.Id, created.Id), ct);
            }

            document.StatusId = CoreConstants.MarketDocumentStatuses.Done;
        }
        catch (Exception)
        {
            document.StatusId = CoreConstants.MarketDocumentStatuses.Error;
        }

        await documentRepository.UpdateAsync(document, ct);
    }
}
