using System.Globalization;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class InitialMeterRead_867_04_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : TransactionXmlReaderBase<UsageTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = "BigData_867_04";
    private const string InitialUsageType = "INT";
    protected override string RequiredElementName => "Usage";

    public async Task<Result> CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transactionResult = DeserializeTransaction(item);
        if (!transactionResult.IsSuccess)
        {
            return AsResult(transactionResult);
        }

        var usage = transactionResult.Value.Usage;

        var serviceStartResult = await GetServiceStartAsync(usage.TranNr814, ct);
        if (!serviceStartResult.IsSuccess)
        {
            return AsResult(serviceStartResult);
        }

        var serviceStart = serviceStartResult.Value;

        foreach (var usageMeter in usage.UsageMeters.Where(x => x.UsageType == InitialUsageType))
        {
            var serviceDate = DateOnly.ParseExact(usageMeter.SwitchDate ?? string.Empty, DateFormat, CultureInfo.InvariantCulture);

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

                    var reference = new MarketDocumentReference
                    {
                        MarketDocumentItemId = item.Id,
                        ReferenceTypeId = Constants.MarketDocumentReferenceTypes.ServiceStart,
                        ReferenceId = created.Id,
                    };

                    await referenceRepository.AddAsync(reference, ct);
                }
            }

            // TODO: move this to an event handler
            serviceStart.ServiceDate = serviceDate;
            serviceStart.ServiceOrderStatusId = Constants.ServiceOrderStatuses.Completed;
        }

        return Result.Success();
    }

    private async Task<Result<ServiceStart>> GetServiceStartAsync(string? sourceReferenceNumber, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(sourceReferenceNumber))
        {
            return Result.Invalid(new ValidationError(nameof(sourceReferenceNumber), "Transaction has no TranNr814."));
        }

        var serviceStartItem = await serviceStartItemRepository.FirstOrDefaultAsync(
            new ServiceStartItemByIdentifierSpec(sourceReferenceNumber), ct);

        return serviceStartItem is not null
            ? Result.Success(serviceStartItem.ServiceStart)
            : Result.Invalid(new ValidationError(nameof(sourceReferenceNumber), $"ServiceStartItem with tracking number '{sourceReferenceNumber}' was not found."));
    }
}
