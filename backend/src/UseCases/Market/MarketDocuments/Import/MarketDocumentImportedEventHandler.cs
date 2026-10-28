using Core.MarketExchange;
using Core.MarketExchange.Events;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UseCases.Market.MarketDocuments.Import;

public class MarketDocumentImportedEventHandler(
    IRepository<MarketDocumentItem> itemRepository,
    IRepository<MarketDocument> documentRepository,
    IReadRepository<Configuration> configurationRepository,
    EdiImportHandlerResolver ediResolver)
{
    private const string HandlerIdentifier = "handler";

    public async Task Handle(MarketDocumentImportedEvent domainEvent, CancellationToken ct)
    {
        var document = domainEvent.MarketDocument;

        try
        {
            var configuration = await configurationRepository.FirstOrDefaultAsync(
                new ConfigurationByTypeAndCompanySpec(Constants.ConfigurationTypes.Edi, document.CompanyId), ct)
                ?? throw new InvalidOperationException($"No EDI configuration was found for company '{document.CompanyId}'.");

            var ediHandler = ediResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));

            var items = await ediHandler.ExecuteItemImportAsync(configuration, document, ct);

            foreach (var item in items)
            {
                item.MarketDocumentId = document.Id;
                item.MarketDocument = document;
                item.MarkImported();

                await itemRepository.AddAsync(item, ct);
            }

            document.StatusId = Constants.MarketDocumentStatuses.Done;
        }
        catch (Exception)
        {
            document.StatusId = Constants.MarketDocumentStatuses.Error;
        }

        await documentRepository.UpdateAsync(document, ct);
    }
}
