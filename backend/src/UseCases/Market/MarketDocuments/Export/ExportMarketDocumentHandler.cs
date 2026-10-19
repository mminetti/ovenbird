using Core.MarketExchange;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Export.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace UseCases.Market.MarketDocuments.Export;

public record ExportMarketDocumentCommand();

public class ExportMarketDocumentHandler(
    IRepository<MarketDocument> documentRepository,
    IRepository<MarketDocumentItem> itemRepository,
    IReadRepository<Configuration> configurationReadRepository,
    EdiExportHandlerResolver ediResolver)
{
    private const string HandlerIdentifier = "handler";

    public async Task<Result<IReadOnlyList<long>>> Handle(ExportMarketDocumentCommand command, CancellationToken ct)
    {
        var documentIds = new List<long>();

        var configurations = await configurationReadRepository.ListAsync(
            new ConfigurationByTypeSpec(CoreConstants.ConfigurationTypes.Edi), ct);

        foreach (var configuration in configurations)
        {
            var ediHandler = ediResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));

            var export = await ediHandler.ExecuteExportAsync(configuration, ct);

            if (export is null)
            {
                continue;
            }

            var created = await documentRepository.AddAsync(export.Document, ct);

            foreach (var item in export.Items)
            {
                item.MarketDocumentId = created.Id;
                await itemRepository.UpdateAsync(item, ct);
            }

            documentIds.Add(created.Id);
        }

        return Result.Success<IReadOnlyList<long>>(documentIds);
    }
}
