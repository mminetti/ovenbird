using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using UseCases.Interfaces.ServiceDelivery;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public abstract class ServiceStartRejectImportBuilderBase(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IServiceStartService serviceStartService)
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

        var serviceStartResult = await serviceStartService.GetByIdentifierAsync(accountMaint.ProcessTrackingNumber, ct);
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

        serviceStart.ServiceOrderStatusId = Constants.ServiceOrderStatuses.Rejected;

        return Result.Success<IReadOnlyList<long>>([created.Id]);
    }

    private static Result<ServiceXml> ValidateAccountMaint(AccountMaintXml accountMaint, long itemId)
    {
        var service = accountMaint.Services.FirstOrDefault();

        return service is not null
            ? Result.Success(service)
            : Result.Invalid(new ValidationError(nameof(accountMaint.Services), $"MarketDocumentItem '{itemId}' has no Service."));
    }
}
