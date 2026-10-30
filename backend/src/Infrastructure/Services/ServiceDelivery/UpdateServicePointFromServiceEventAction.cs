using Core.Common.Constants;
using Core.MarketExchange;
using Core.MarketExchange.Specifications;
using Core.ServiceDelivery;
using Core.Settings;
using Core.Settings.Specifications;
using UseCases.Interfaces.ServiceDelivery;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.ServiceDelivery;

public class UpdateServicePointFromServiceEventAction(
    IReadRepository<ServiceEvent> serviceEventRepository,
    IReadRepository<MarketDocumentReference> referenceRepository,
    IReadRepository<Configuration> configurationRepository,
    ServicePointUpdateBuilderResolver builderResolver,
    IUpdateServicePointAction updateServicePointAction)
    : IUpdateServicePointFromServiceEventAction
{
    private const string HandlerIdentifierField = "handler";

    public async Task<Result> ExecuteAsync(long serviceEventId, CancellationToken ct)
    {
        var serviceEvent = await serviceEventRepository.GetByIdAsync(serviceEventId, ct);
        if (serviceEvent is null)
        {
            return Result.Invalid(new ValidationError(nameof(ServiceEvent), $"ServiceEvent '{serviceEventId}' was not found."));
        }

        var reference = await referenceRepository.FirstOrDefaultAsync(
            new MarketDocumentReferenceByReferenceSpec(Constants.MarketDocumentReferenceTypes.ServiceStart, serviceEvent.ReferenceId), ct);

        var item = reference?.MarketDocumentItem;
        if (item is null)
        {
            return Result.Invalid(new ValidationError(
                nameof(MarketDocumentItem), $"MarketDocumentItem for ServiceEvent '{serviceEventId}' was not found."));
        }

        var companyId = item.MarketDocument?.CompanyId;
        if (companyId is null)
        {
            return Result.Invalid(new ValidationError(
                nameof(MarketDocument), $"MarketDocumentItem '{item.Id}' has no associated MarketDocument."));
        }

        var configuration = await configurationRepository.FirstOrDefaultAsync(
            new ConfigurationByTypeAndCompanySpec(Constants.ConfigurationTypes.Edi, companyId.Value), ct);

        if (configuration is null)
        {
            return Result.Invalid(new ValidationError(
                nameof(Configuration), $"No EDI configuration was found for company '{companyId}'."));
        }

        string handlerIdentifier;
        try
        {
            handlerIdentifier = configuration.GetRequiredValue(HandlerIdentifierField);
        }
        catch (InvalidOperationException ex)
        {
            return Result.Invalid(new ValidationError(nameof(Configuration), ex.Message));
        }

        var builderKey = $"{handlerIdentifier}_{item.Set}_{item.SubSet}";
        var builder = builderResolver.Resolve(builderKey);

        if (builder is null)
        {
            return Result.Invalid(new ValidationError(
                nameof(IServicePointUpdateBuilder), $"No ServicePoint update builder was found for '{builderKey}'."));
        }

        var requestResult = await builder.BuildAsync(item, ct);
        if (!requestResult.IsSuccess)
        {
            return Result.Invalid(requestResult.ValidationErrors.ToList());
        }

        return await updateServicePointAction.ExecuteAsync(requestResult.Value, ct);
    }
}
