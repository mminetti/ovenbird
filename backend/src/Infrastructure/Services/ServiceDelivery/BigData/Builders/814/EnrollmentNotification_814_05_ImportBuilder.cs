using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using UseCases.Interfaces.ServiceDelivery;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class EnrollmentNotification_814_05_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<ServiceStart> serviceStartRepository,
    IRepository<ServiceEvent> serviceEventRepository,
    IUpdateServicePointService updateServicePointService)
    : TransactionXmlReaderBase<ServiceRequestTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = $"BigData_{Set}_{SubSet}";
    private const string Set = "814";
    private const string SubSet = "05";
    private const string PremiseEntityType = "8R";
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
        var accountValidationResult = ValidateAccountMaint(accountMaint);

        if (!serviceStartResult.IsSuccess || !accountValidationResult.IsSuccess)
        {
            return Result<IReadOnlyList<long>>.Invalid(
                serviceStartResult.ValidationErrors.Concat(accountValidationResult.ValidationErrors).ToList());
        }

        var serviceStart = serviceStartResult.Value;
        var service = accountMaint.Services.First();
        var serviceDate = accountValidationResult.Value;

        var premise = accountMaint.NameAddresses.FirstOrDefault(x => x.EntityType == PremiseEntityType);
        var serviceMeter = service.ServiceMeters.FirstOrDefault();

        var updateServicePointRequest = new UpdateServicePointRequest(
            service.UtilityAccountNumber,
            transaction.Commodity,
            serviceMeter?.LDCRateClass,
            serviceMeter?.LoadProfileCode,
            serviceMeter?.OwnerMeterCycle,
            serviceMeter?.AdvanceMeterInfo,
            serviceMeter?.MeterNumber,
            premise?.EntityAddress1,
            premise?.EntityCity,
            premise?.EntityState,
            premise?.EntityPostalCode,
            premise?.EntityCounty);

        var updateServicePointResult = await updateServicePointService.ExecuteAsync(updateServicePointRequest, ct);
        if (!updateServicePointResult.IsSuccess)
        {
            return Result<IReadOnlyList<long>>.Invalid(updateServicePointResult.ValidationErrors.ToList());
        }

        var serviceStartItem = new ServiceStartItem
        {
            ServiceStartId = serviceStart.Id,
            Identifier = item.ReferenceNumber,
            ServiceActionId = Constants.ServiceActions.Confirm,
        };

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

    private static Result<DateOnly> ValidateAccountMaint(AccountMaintXml accountMaint)
    {
        var service = accountMaint.Services.FirstOrDefault();
        if (service is null)
        {
            return Result.Invalid(new ValidationError(nameof(accountMaint.Services), "Service is required."));
        }

        var customerAgreementDate = service.ServiceDates.FirstOrDefault()?.CustomerAgreementDate;
        if (customerAgreementDate is null)
        {
            return Result.Invalid(new ValidationError(nameof(customerAgreementDate), "Service Start Date is required."));
        }

        return DateOnly.TryParseExact(customerAgreementDate, DateFormat, out var serviceDate)
            ? Result.Success(serviceDate)
            : Result.Invalid(new ValidationError(nameof(customerAgreementDate), "Service Start Date is invalid."));
    }
}
