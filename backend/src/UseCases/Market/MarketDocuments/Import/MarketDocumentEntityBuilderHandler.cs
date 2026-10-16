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

    public async Task Handle(MarketDocumentEntityBuilderCommand command, CancellationToken ct)
    {
        var item = await itemReadRepository.GetByIdAsync(command.MarketDocumentItemId, ct)
            ?? throw new InvalidOperationException($"MarketDocumentItem '{command.MarketDocumentItemId}' was not found.");

        var configuration = await configurationReadRepository.FirstOrDefaultAsync(
            new ConfigurationByIdSpec(command.ConfigurationId), ct)
            ?? throw new InvalidOperationException($"Configuration '{command.ConfigurationId}' was not found.");

        var handlerIdentifier = configuration.GetRequiredValue(HandlerIdentifier);
        var entityBuilder = entityBuilderResolver.Resolve($"{handlerIdentifier}_{item.Set}_{item.SubSet}");

        if (entityBuilder is not null)
        {
            await entityBuilder.CreateAsync(item, ct);
        }
    }
}
