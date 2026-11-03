using System.Globalization;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using UseCases.Interfaces.ServiceDelivery;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class InitialMeterRead_867_04_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IServiceStartService serviceStartService)
    : TransactionXmlReaderBase<UsageTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = "BigData_867_04";
    private const string InitialUsageType = "INT";
    protected override string RequiredElementName => "Usage";

    public int EventTypeId => Constants.MarketDocumentEventTypes.ServiceStart;

    public async Task<Result<IReadOnlyList<long>>> CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transactionResult = DeserializeTransaction(item);
        if (!transactionResult.IsSuccess)
        {
            return AsResult(transactionResult);
        }

        var usage = transactionResult.Value.Usage;

        var serviceStartResult = await serviceStartService.GetByIdentifierAsync(usage.TranNr814, ct);
        if (!serviceStartResult.IsSuccess)
        {
            return AsResult(serviceStartResult);
        }

        var serviceStart = serviceStartResult.Value;
        var createdIds = new List<long>();

        foreach (var usageMeter in usage.UsageMeters.Where(x => x.UsageType == InitialUsageType))
        {
            if (!DateOnly.TryParseExact(usageMeter.SwitchDate, DateFormat, out var serviceDate))
            {
                return Result<IReadOnlyList<long>>.Invalid(new ValidationError(
                    nameof(usageMeter.SwitchDate), $"Switch Date '{usageMeter.SwitchDate}' is invalid."));
            }

            foreach (var quantity in usageMeter.UsageQuantities)
            {
                foreach (var read in quantity.UsageReads)
                {
                    var serviceStartItem = new ServiceStartItem
                    {
                        ServiceStartId = serviceStart.Id,
                        Identifier = item.ReferenceNumber,
                        ServiceActionId = Constants.ServiceActions.Complete,
                    };

                    var created = await serviceStartItemRepository.AddAsync(serviceStartItem, ct);
                    createdIds.Add(created.Id);
                }
            }

            serviceStart.ServiceDate = serviceDate;
            serviceStart.ServiceOrderStatusId = Constants.ServiceOrderStatuses.Completed;
        }

        return Result.Success<IReadOnlyList<long>>(createdIds);
    }
}
