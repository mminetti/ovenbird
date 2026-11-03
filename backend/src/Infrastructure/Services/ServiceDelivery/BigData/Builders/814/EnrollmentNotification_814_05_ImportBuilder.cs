using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class EnrollmentNotification_814_05_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<ServiceStart> serviceStartRepository,
    IRepository<ServiceEvent> serviceEventRepository)
    : TransactionXmlReaderBase<ServiceRequestTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = $"BigData_{Set}_{SubSet}";
    private const string Set = "814";
    private const string SubSet = "05";
    protected override string RequiredElementName => "AccountMaint";

    public int EventTypeId => Constants.MarketDocumentEventTypes.ServiceStart;

    public async Task<Result<IReadOnlyList<long>>> CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        // TODO: refactor this

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

        var serviceStartItem = new ServiceStartItem
        {
            ServiceStartId = serviceStart.Id,
            Identifier = item.ReferenceNumber,
            ServiceActionId = Constants.ServiceActions.Confirm,
        };

        var service = accountMaint.Services.FirstOrDefault();
        if (service is null)
        {
            return Result<IReadOnlyList<long>>.Invalid(new ValidationError(nameof(accountMaint.Services), $"Service is required."));
        }

        var customerAgreementDate = service.ServiceDates.FirstOrDefault()?.CustomerAgreementDate;

        if (customerAgreementDate is null)
        {
            return Result<IReadOnlyList<long>>.Invalid(new ValidationError(nameof(customerAgreementDate),
                $"Service Start Date is required."));
        }

        if (!DateOnly.TryParseExact(customerAgreementDate, DateFormat, out var serviceDate))
        {
            return Result<IReadOnlyList<long>>.Invalid(new ValidationError(nameof(customerAgreementDate),
                $"Service Start Date is invalid."));
        }

        var created = await serviceStartItemRepository.AddAsync(serviceStartItem, ct);

        var serviceEvent = new ServiceEvent
        {
            EventTypeId = Constants.ServiceEventTypes.ServiceStartConfirmed,
            ReferenceTypeId = Constants.ServiceEventReferenceTypes.ServiceStartItem,
            ReferenceId = created.Id,
        };

        var createdServiceEvent = await serviceEventRepository.AddAsync(serviceEvent, ct);

        createdServiceEvent.MarkCreated();
        await serviceEventRepository.UpdateAsync(createdServiceEvent, ct);

        serviceStart.MarkScheduled(serviceDate);
        await serviceStartRepository.UpdateAsync(serviceStart, ct);

        return Result.Success<IReadOnlyList<long>>([created.Id]);
    }

    private async Task<Result<ServiceStart>> GetServiceStartAsync(string? identifier, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return Result.Invalid(new ValidationError(nameof(ServiceStart), "Service Start has no Identifier."));
        }

        var serviceStartItem = await serviceStartItemRepository.FirstOrDefaultAsync(
            new ServiceStartItemByIdentifierSpec(identifier), ct);

        return serviceStartItem is not null
            ? Result.Success(serviceStartItem.ServiceStart)
            : Result.Invalid(new ValidationError(nameof(ServiceStart), $"Service Start Item with identifier '{identifier}' was not found."));
    }
}
