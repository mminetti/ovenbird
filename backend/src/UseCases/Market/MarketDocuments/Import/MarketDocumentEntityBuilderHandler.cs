using Core.MarketExchange;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace UseCases.Market.MarketDocuments.Import;

public record MarketDocumentEntityBuilderCommand(int ConfigurationId, long MarketDocumentItemId);

public class MarketDocumentEntityBuilderHandler(
    IReadRepository<MarketDocumentItem> itemReadRepository,
    IReadRepository<Configuration> configurationReadRepository,
    MarketDocumentEntityBuilderResolver entityBuilderResolver)
{
    private const string HandlerIdentifier = "handler";

    public async Task<Result> Handle(MarketDocumentEntityBuilderCommand command, CancellationToken ct)
    {
        var item = await itemReadRepository.GetByIdAsync(command.MarketDocumentItemId, ct);

        if (item is null)
        {
            return Result.NotFound($"Market Document not found for id {command.MarketDocumentItemId}");
        }

        var configuration = await configurationReadRepository.FirstOrDefaultAsync(
            new ConfigurationByIdSpec(command.ConfigurationId), ct);

        if (configuration is null)
        {
            return Result.NotFound($"Configuration not found for id {command.ConfigurationId}");
        }

        try
        {
            var handlerIdentifier = configuration.GetRequiredValue(HandlerIdentifier);
            var entityBuilder = entityBuilderResolver.Resolve($"{handlerIdentifier}_{item.Set}_{item.SubSet}");

            return entityBuilder is not null
                ? await entityBuilder.CreateAsync(item, ct)
                : Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Invalid(new ValidationError(item.ReferenceNumber, ex.Message));
        }
    }
}
