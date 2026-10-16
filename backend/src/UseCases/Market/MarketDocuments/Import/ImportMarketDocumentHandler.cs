using Core.MarketExchange;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace UseCases.Market.MarketDocuments.Import;

public record ImportMarketDocumentCommand();

public class ImportMarketDocumentHandler(
    IRepository<MarketDocument> documentRepository,
    IReadRepository<Configuration> configurationReadRepository,
    EdiImportHandlerResolver ediResolver,
    IMessageBus bus)
{
    private const string HandlerIdentifier = "handler";

    public async Task<Result<IReadOnlyList<long>>> Handle(ImportMarketDocumentCommand command, CancellationToken ct)
    {
        var documentIds = new List<long>();

        var configurations = await configurationReadRepository.ListAsync(
            new ConfigurationByTypeSpec(CoreConstants.ConfigurationTypes.Edi), ct);

        foreach (var configuration in configurations)
        {
            var company = configuration.GetRequiredCompany();
            var ediHandler = ediResolver.Resolve(configuration.GetRequiredValue(HandlerIdentifier));

            var transactions = await ediHandler.ListTransactionsAsync(configuration, ct);

            foreach (var transaction in transactions)
            {
                var existing = await ediHandler.FindExistingTransactionAsync(configuration, transaction, ct);

                if (existing is not null)
                {
                    documentIds.Add(existing.Id);
                    continue;
                }

                var stored = await ediHandler.StoreTransactionAsync(configuration, transaction, ct);

                var document = new MarketDocument
                {
                    Name = stored.Name,
                    File = stored.FileReference,
                    CompanyId = company.Id,
                    DirectionId = CoreConstants.MarketDocumentDirections.Inbound,
                    StatusId = CoreConstants.MarketDocumentStatuses.New
                };

                var created = await documentRepository.AddAsync(document, ct);

                _ = await bus.InvokeAsync<Result<IReadOnlyList<long>>>(
                    new ImportMarketDocumentItemCommand(configuration.Id, created.Id), ct);

                documentIds.Add(created.Id);
            }
        }

        return Result.Success<IReadOnlyList<long>>(documentIds);
    }
}
