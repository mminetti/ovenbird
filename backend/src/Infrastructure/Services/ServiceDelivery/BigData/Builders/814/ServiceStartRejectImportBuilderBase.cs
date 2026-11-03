using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public abstract class ServiceStartRejectImportBuilderBase(
    IRepository<ServiceStartItem> serviceStartItemRepository)
    : TransactionXmlReaderBase<ServiceRequestTransactionXml>, IMarketDocumentEntityBuilder
{
    protected override string RequiredElementName => "AccountMaint";

    public int EventTypeId => Constants.MarketDocumentEventTypes.ServiceStart;

    public async Task<Result<IReadOnlyList<long>>> CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transactionResult = DeserializeTransaction(item);
        if (!transactionResult.IsSuccess)
        {
            return AsResult(transactionResult);
        }

        var transaction = transactionResult.Value;
        var accountMaint = transaction.AccountMaint;

        var serviceStartResult = await GetServiceStartAsync(accountMaint.ProcessTrackingNumber, ct);
        var serviceResult = ValidateAccountMaint(accountMaint, item.Id);

        if (!serviceStartResult.IsSuccess || !serviceResult.IsSuccess)
        {
            return Result<IReadOnlyList<long>>.Invalid(
                serviceStartResult.ValidationErrors.Concat(serviceResult.ValidationErrors).ToList());
        }

        var serviceStart = serviceStartResult.Value;
        var service = serviceResult.Value;

        var rejectReason = service.ServiceRejectReasons.FirstOrDefault();

        var serviceStartItem = new ServiceStartItem
        {
            ServiceStartId = serviceStart.Id,
            Identifier = item.ReferenceNumber,
            ServiceActionId = Constants.ServiceActions.Reject,
            Description = $"{rejectReason?.RejectCode} - {rejectReason?.RejectReason}",
        };

        var created = await serviceStartItemRepository.AddAsync(serviceStartItem, ct);

        // TODO:
        serviceStart.ServiceOrderStatusId = Constants.ServiceOrderStatuses.Rejected;

        return Result.Success<IReadOnlyList<long>>([created.Id]);
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

    private static Result<ServiceXml> ValidateAccountMaint(AccountMaintXml accountMaint, long itemId)
    {
        var service = accountMaint.Services.FirstOrDefault();

        return service is not null
            ? Result.Success(service)
            : Result.Invalid(new ValidationError(nameof(accountMaint.Services), $"MarketDocumentItem '{itemId}' has no Service."));
    }
}
