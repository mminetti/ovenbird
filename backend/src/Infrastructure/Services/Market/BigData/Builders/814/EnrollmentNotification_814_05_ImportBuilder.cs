using System.Globalization;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;
using Core.ServiceDelivery.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using Wolverine;

namespace Infrastructure.Services.Market.BigData.Builders;

public class EnrollmentNotification_814_05_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository,
    IReadRepository<CustomerClass> customerClassRepository,
    IReadRepository<LoadProfile> loadProfileRepository,
    IReadRepository<ReadCycle> readCycleRepository,
    IReadRepository<County> countyRepository,
    IBigDataValueMapper valueMapper,
    IMessageBus bus)
    : TransactionXmlReaderBase<ServiceRequestTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = "BigData_814_05";
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

        var countyIdResult = await GetCountyIdAsync(premiseAddress.EntityCounty, ct);
        if (!countyIdResult.IsSuccess)
        {
            return AsResult(countyIdResult);
        }

        var countyId = countyIdResult.Value;

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

        var customerClassIdResult = await GetCustomerClassIdAsync(serviceMeter.LDCRateClass, ct);
        if (!customerClassIdResult.IsSuccess)
        {
            return AsResult(customerClassIdResult);
        }

        var customerClassId = customerClassIdResult.Value;

        var loadProfileIdResult = await GetLoadProfileIdAsync(serviceMeter.LoadProfileCode, ct);
        if (!loadProfileIdResult.IsSuccess)
        {
            return AsResult(loadProfileIdResult);
        }

        var loadProfileId = loadProfileIdResult.Value;

        var readCycleIdResult = await GetReadCycleIdAsync(serviceMeter.OwnerMeterCycle, ct);
        if (!readCycleIdResult.IsSuccess)
        {
            return AsResult(readCycleIdResult);
        }

        var readCycleId = readCycleIdResult.Value;

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
            customerClassId,
            loadProfileId,
            readCycleId,
            isAmsMeter,
            serviceMeter.MeterNumber ?? string.Empty,
            premiseAddress.EntityAddress1 ?? string.Empty,
            premiseAddress.EntityCity ?? string.Empty,
            premiseAddress.EntityState ?? string.Empty,
            zip,
            zip4,
            countyId));

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

    private async Task<Result<int>> GetCustomerClassIdAsync(string? name, CancellationToken ct)
    {
        var customerClass = await customerClassRepository.FirstOrDefaultAsync(new CustomerClassByNameSpec(name ?? string.Empty), ct);

        return customerClass is not null
            ? Result.Success(customerClass.Id)
            : Result.Invalid(new ValidationError(nameof(CustomerClass), $"Unmapped customer class '{name}'."));
    }

    private async Task<Result<int>> GetLoadProfileIdAsync(string? name, CancellationToken ct)
    {
        var loadProfile = await loadProfileRepository.FirstOrDefaultAsync(new LoadProfileByNameSpec(name ?? string.Empty), ct);

        return loadProfile is not null
            ? Result.Success(loadProfile.Id)
            : Result.Invalid(new ValidationError(nameof(LoadProfile), $"Unmapped load profile '{name}'."));
    }

    private async Task<Result<int>> GetReadCycleIdAsync(string? name, CancellationToken ct)
    {
        var readCycle = await readCycleRepository.FirstOrDefaultAsync(new ReadCycleByNameSpec(name ?? string.Empty), ct);

        return readCycle is not null
            ? Result.Success(readCycle.Id)
            : Result.Invalid(new ValidationError(nameof(ReadCycle), $"Unmapped read cycle '{name}'."));
    }

    private async Task<Result<int>> GetCountyIdAsync(string? name, CancellationToken ct)
    {
        var county = await countyRepository.FirstOrDefaultAsync(new CountyByNameSpec(name ?? string.Empty), ct);

        return county is not null
            ? Result.Success(county.Id)
            : Result.Invalid(new ValidationError(nameof(County), $"Unmapped county '{name}'."));
    }

    private static (string Zip, string? Zip4) SplitZip(string? postalCode)
    {
        var value = postalCode ?? string.Empty;

        return value.Length == 9
            ? (value[..5], value[5..])
            : (value, null);
    }
}
