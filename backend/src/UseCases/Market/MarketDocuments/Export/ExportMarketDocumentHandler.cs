using Core.MarketExchange;
using Core.MarketExchange.Specifications;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Export.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace UseCases.Market.MarketDocuments.Export;

public record ExportMarketDocumentCommand();

public class ExportMarketDocumentHandler(
    IRepository<MarketDocument> documentRepository,
    IRepository<MarketDocumentItem> itemRepository,
    IReadRepository<MarketDocumentItem> itemReadRepository,
    IReadRepository<Configuration> configurationReadRepository,
    EdiExportHandlerResolver ediResolver,
    MarketDocumentGeneratorResolver generatorResolver,
    TimeProvider timeProvider)
{
    private const string HandlerIdentifier = "handler";
    private const string Set = "814";

    public async Task<Result<IReadOnlyList<long>>> Handle(ExportMarketDocumentCommand command, CancellationToken ct)
    {
        var documentIds = new List<long>();

        var configurations = await configurationReadRepository.ListAsync(
            new ConfigurationByTypeSpec(CoreConstants.ConfigurationTypes.Edi), ct);

        foreach (var configuration in configurations)
        {
            var pendingItems = await itemReadRepository.ListAsync(
                new MarketDocumentItemsPendingExportSpec(Set), ct);

            if (pendingItems.Count == 0)
            {
                continue;
            }

            var company = configuration.GetRequiredCompany();
            var generator = generatorResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));
            var ediHandler = ediResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));

            var fileName = $"{configuration.Name}_{timeProvider.GetUtcNow():yyyyMMddHHmmss}.xml";

            using var content = await generator.WriteTransactionsAsync(pendingItems, ct);

            var uploadedFileReference = await ediHandler.StoreTransactionAsync(configuration, content, fileName, ct);

            var document = new MarketDocument
            {
                Name = fileName,
                File = uploadedFileReference,
                CompanyId = company.Id,
                DirectionId = CoreConstants.MarketDocumentDirections.Outbound,
                StatusId = CoreConstants.MarketDocumentStatuses.New,
            };

            var created = await documentRepository.AddAsync(document, ct);

            foreach (var item in pendingItems)
            {
                item.MarketDocumentId = created.Id;
                await itemRepository.UpdateAsync(item, ct);
            }

            documentIds.Add(created.Id);
        }

        return Result.Success<IReadOnlyList<long>>(documentIds);
    }
}
