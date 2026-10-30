using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class EnrollmentNotification_814_05_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository,
    IRepository<ServiceStart> serviceStartRepository,
    IRepository<ServiceEvent> serviceEventRepository)
    : TransactionXmlReaderBase<ServiceRequestTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = $"BigData_{Set}_{SubSet}";
    private const string Set = "814";
    private const string SubSet = "05";
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

        var serviceStartItem = new ServiceStartItem
        {
            ServiceStartId = serviceStart.Id,
            Identifier = item.ReferenceNumber,
            ServiceActionId = Constants.ServiceActions.Confirm,
        };

        var service = accountMaint.Services.FirstOrDefault();
        if (service is null)
        {
            return Result.Invalid(new ValidationError(nameof(accountMaint.Services), $"Service is required."));
        }

        var customerAgreementDate = service.ServiceDates.FirstOrDefault()?.CustomerAgreementDate;

        if (customerAgreementDate is null)
        {
            return Result.Invalid(new ValidationError(nameof(customerAgreementDate),
                $"Service Start Date is required."));
        }

        if (!DateOnly.TryParseExact(customerAgreementDate, DateFormat, out var serviceDate))
        {
            return Result.Invalid(new ValidationError(nameof(customerAgreementDate),
                $"Service Start Date is invalid."));
        }

        var created = await serviceStartItemRepository.AddAsync(serviceStartItem, ct);

        var reference = new MarketDocumentReference
        {
            MarketDocumentItemId = item.Id,
            ReferenceTypeId = Constants.MarketDocumentReferenceTypes.ServiceStart,
            ReferenceId = created.Id,
        };

        await referenceRepository.AddAsync(reference, ct);

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

        return Result.Success();
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
