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

        var entityReference = EntityReference.Format(nameof(MarketDocumentItem), item.Id);

        try
        {
            var handlerIdentifier = configuration.GetRequiredValue(HandlerIdentifier);
            var entityBuilder = entityBuilderResolver.Resolve($"{handlerIdentifier}_{item.Set}_{item.SubSet}");

            var result = entityBuilder is not null
                ? await entityBuilder.CreateAsync(item, ct)
                : Result.Success();

            // The builder's ValidationErrors already identify the failing field (via nameof(...)) -
            // promote that into ErrorCode as the "fix action" field key, and replace Identifier with
            // the record this error is actually about, now that we know it.
            return result.Status == ResultStatus.Invalid
                ? Result.Invalid(result.ValidationErrors.Select(v =>
                    new ValidationError(entityReference, v.ErrorMessage, v.Identifier, v.Severity)))
                : result;
        }
        catch (Exception ex)
        {
            return Result.Invalid(new ValidationError(entityReference, ex.Message, string.Empty, ValidationSeverity.Error));
        }
    }
}
