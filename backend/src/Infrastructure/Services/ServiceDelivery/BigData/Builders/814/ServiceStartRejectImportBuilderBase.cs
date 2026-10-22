using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public abstract class ServiceStartRejectImportBuilderBase(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : TransactionXmlReaderBase<ServiceRequestTransactionXml>, IMarketDocumentEntityBuilder
{
    protected override string RequiredElementName => "AccountMaint";

    public async Task<Result> CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transactionResult = DeserializeTransaction(item);
        if (!transactionResult.IsSuccess)
        {
            return AsResult(transactionResult);
        }

        var transaction = transactionResult.Value;
        var accountMaint = transaction.AccountMaint;

        var serviceStartResult = await GetServiceStartAsync(accountMaint.ProcessTrackingNumber, ct);
        if (!serviceStartResult.IsSuccess)
        {
            return AsResult(serviceStartResult);
        }

        var serviceStart = serviceStartResult.Value;

        var service = accountMaint.Services.FirstOrDefault();
        if (service is null)
        {
            return Result.Invalid(new ValidationError(
                nameof(accountMaint.Services),
                $"MarketDocumentItem '{item.Id}' has no Service."));
        }

        var rejectReason = service.ServiceRejectReasons.FirstOrDefault();

        var serviceStartItem = new ServiceStartItem
        {
            ServiceStartId = serviceStart.Id,
            Identifier = item.ReferenceNumber,
            ServiceActionId = Constants.ServiceActions.Reject,
            Description = $"{rejectReason?.RejectCode} - {rejectReason?.RejectReason}",
        };

        var created = await serviceStartItemRepository.AddAsync(serviceStartItem, ct);

        var reference = new MarketDocumentReference
        {
            MarketDocumentItemId = item.Id,
            ReferenceTypeId = Constants.MarketDocumentReferenceTypes.ServiceStart,
            ReferenceId = created.Id,
        };

        await referenceRepository.AddAsync(reference, ct);

        // TODO:
        serviceStart.ServiceOrderStatusId = Constants.ServiceOrderStatuses.Rejected;

        return Result.Success();
    }

    private async Task<Result<ServiceStart>> GetServiceStartAsync(string? trackingNumber, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
        {
            return Result.Invalid(new ValidationError(nameof(trackingNumber), "Transaction has no ProcessTrackingNumber."));
        }

        var serviceStartItem = await serviceStartItemRepository.FirstOrDefaultAsync(
            new ServiceStartItemByIdentifierSpec(trackingNumber), ct);

        return serviceStartItem is not null
            ? Result.Success(serviceStartItem.ServiceStart)
            : Result.Invalid(new ValidationError(nameof(trackingNumber), $"ServiceStartItem with tracking number '{trackingNumber}' was not found."));
    }
}
