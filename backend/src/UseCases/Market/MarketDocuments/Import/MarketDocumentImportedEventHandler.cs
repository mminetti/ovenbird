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

            var ediImportHandler = ediResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));

            var marketDocumentItems = await ediImportHandler.ExecuteItemImportAsync(configuration, document, ct);

            foreach (var item in marketDocumentItems)
            {
                item.MarketDocumentId = document.Id;
                item.MarketDocument = document;

                if (item.MarketDocumentItemStatusId == Constants.MarketDocumentItemStatuses.New)
                {
                    item.MarkImported();
                }

                await itemRepository.AddAsync(item, ct);
            }

            document.StatusId = Constants.MarketDocumentStatuses.Done;
        }
        catch (Exception ex)
        {
            document.StatusId = Constants.MarketDocumentStatuses.Error;
            document.Message = ex.Message;
        }

        await documentRepository.UpdateAsync(document, ct);
    }
}
