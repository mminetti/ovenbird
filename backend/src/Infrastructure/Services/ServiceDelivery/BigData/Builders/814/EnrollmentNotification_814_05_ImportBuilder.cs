using System.Globalization;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;
using Core.ServiceDelivery.Specifications;
using UseCases.Interfaces.ServiceDelivery;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using Wolverine;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class EnrollmentNotification_814_05_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository,
    IServiceDeliveryLookupService lookupService,
    IBigDataValueMapper valueMapper,
    IMessageBus bus)
    : TransactionXmlReaderBase<ServiceRequestTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = $"BigData_{Set}_{SubSet}";
    private const string Set = "814";
    private const string SubSet = "05";
    private const string PremiseEntityType = "8R";
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

        var commodityIdResult = valueMapper.GetCommodityId(transaction.Commodity);
        if (!commodityIdResult.IsSuccess)
        {
            return AsResult(commodityIdResult);
        }

        var commodityId = commodityIdResult.Value;

        var premiseAddress = accountMaint.NameAddresses.FirstOrDefault(x => x.EntityType == PremiseEntityType);
        if (premiseAddress is null)
        {
            return Result.Invalid(new ValidationError(
                PremiseEntityType,
                $"MarketDocumentItem '{item.Id}' has no NameAddress with EntityType '{PremiseEntityType}'."));
        }

        var (zip, zip4) = SplitZip(premiseAddress.EntityPostalCode);

        var countyResult = await lookupService.GetCountyByNameAsync(premiseAddress.EntityCounty, ct);
        if (!countyResult.IsSuccess)
        {
            return AsResult(countyResult);
        }

        var county = countyResult.Value;

        var service = accountMaint.Services.FirstOrDefault();
        if (service is null)
        {
            return Result.Invalid(new ValidationError(
                nameof(accountMaint.Services),
                $"MarketDocumentItem '{item.Id}' has no Service."));
        }

        var customerAgreementDate = service.ServiceDates
            .Select(x => x.CustomerAgreementDate)
            .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

        if (customerAgreementDate is null)
        {
            return Result.Invalid(new ValidationError(
                nameof(customerAgreementDate),
                $"MarketDocumentItem '{item.Id}' has no CustomerAgreementDate."));
        }

        var serviceDate = DateOnly.ParseExact(customerAgreementDate, DateFormat, CultureInfo.InvariantCulture);

        var serviceMeter = service.ServiceMeters.FirstOrDefault();
        if (serviceMeter is null)
        {
            return Result.Invalid(new ValidationError(
                nameof(service.ServiceMeters),
                $"MarketDocumentItem '{item.Id}' has no ServiceMeter."));
        }

        var customerClassResult = await lookupService.GetCustomerClassByNameAsync(serviceMeter.LDCRateClass, ct);
        if (!customerClassResult.IsSuccess)
        {
            return AsResult(customerClassResult);
        }

        var customerClass = customerClassResult.Value;

        var loadProfileResult = await lookupService.GetLoadProfileByNameAsync(serviceMeter.LoadProfileCode, ct);
        if (!loadProfileResult.IsSuccess)
        {
            return AsResult(loadProfileResult);
        }

        var loadProfile = loadProfileResult.Value;

        var readCycleResult = await lookupService.GetReadCycleByNameAsync(serviceMeter.OwnerMeterCycle, ct);
        if (!readCycleResult.IsSuccess)
        {
            return AsResult(readCycleResult);
        }

        var readCycle = readCycleResult.Value;

        var isAmsMeter = valueMapper.IsAmsMeter(serviceMeter.AdvanceMeterInfo);

        var serviceStartItem = new ServiceStartItem
        {
            ServiceStartId = serviceStart.Id,
            Identifier = item.ReferenceNumber,
            ServiceActionId = Constants.ServiceActions.Confirm,
        };

        var created = await serviceStartItemRepository.AddAsync(serviceStartItem, ct);

        var reference = new MarketDocumentReference
        {
            MarketDocumentItemId = item.Id,
            ReferenceTypeId = Constants.MarketDocumentReferenceTypes.ServiceStart,
            ReferenceId = created.Id,
        };

        await referenceRepository.AddAsync(reference, ct);

        await bus.PublishAsync(new ServiceStartScheduledEvent(
            serviceStart.Id,
            serviceDate,
            service.UtilityAccountNumber,
            commodityId,
            customerClass.Id,
            loadProfile.Id,
            readCycle.Id,
            isAmsMeter,
            serviceMeter.MeterNumber ?? string.Empty,
            premiseAddress.EntityAddress1 ?? string.Empty,
            premiseAddress.EntityCity ?? string.Empty,
            premiseAddress.EntityState ?? string.Empty,
            zip,
            zip4,
            county.Id));

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
            : Result.Invalid(new ValidationError(nameof(trackingNumber), $"ServiceStartItem with identifier '{trackingNumber}' was not found."));
    }

    private static (string Zip, string? Zip4) SplitZip(string? postalCode)
    {
        var value = postalCode ?? string.Empty;

        return value.Length == 9
            ? (value[..5], value[5..])
            : (value, null);
    }
}
