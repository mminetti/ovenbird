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

    public async Task CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transaction = DeserializeTransaction(item);
        var accountMaint = transaction.AccountMaint;

        var serviceStart = await GetServiceStartAsync(accountMaint.ProcessTrackingNumber, ct);
        var commodityId = valueMapper.GetCommodityId(transaction.Commodity);

        var premiseAddress = accountMaint.NameAddresses
            .FirstOrDefault(x => x.EntityType == PremiseEntityType)
            ?? throw new InvalidOperationException($"MarketDocumentItem '{item.Id}' has no NameAddress with EntityType '{PremiseEntityType}'.");

        var (zip, zip4) = SplitZip(premiseAddress.EntityPostalCode);
        var countyId = await GetCountyIdAsync(premiseAddress.EntityCounty, ct);

        foreach (var service in accountMaint.Services)
        {
            var customerAgreementDate = service.ServiceDates
                .Select(x => x.CustomerAgreementDate)
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))
                ?? throw new InvalidOperationException($"MarketDocumentItem '{item.Id}' has no CustomerAgreementDate.");

            var serviceDate = DateOnly.ParseExact(customerAgreementDate, DateFormat, CultureInfo.InvariantCulture);

            var serviceMeter = service.ServiceMeters.FirstOrDefault()
                ?? throw new InvalidOperationException($"MarketDocumentItem '{item.Id}' has no ServiceMeter.");

            var customerClassId = await GetCustomerClassIdAsync(serviceMeter.LDCRateClass, ct);
            var loadProfileId = await GetLoadProfileIdAsync(serviceMeter.LoadProfileCode, ct);
            var readCycleId = await GetReadCycleIdAsync(serviceMeter.OwnerMeterCycle, ct);
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
        }
    }

    private async Task<ServiceStart> GetServiceStartAsync(string? trackingNumber, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
        {
            throw new InvalidOperationException("Transaction has no ProcessTrackingNumber.");
        }

        var serviceStartItem = await serviceStartItemRepository.FirstOrDefaultAsync(
            new ServiceStartItemByIdentifierSpec(trackingNumber), ct)
            ?? throw new InvalidOperationException($"ServiceStartItem with identifier '{trackingNumber}' was not found.");

        return serviceStartItem.ServiceStart;
    }

    private async Task<int> GetCustomerClassIdAsync(string? name, CancellationToken ct)
    {
        var customerClass = await customerClassRepository.FirstOrDefaultAsync(new CustomerClassByNameSpec(name ?? string.Empty), ct)
            ?? throw new InvalidOperationException($"Unmapped customer class '{name}'.");

        return customerClass.Id;
    }

    private async Task<int> GetLoadProfileIdAsync(string? name, CancellationToken ct)
    {
        var loadProfile = await loadProfileRepository.FirstOrDefaultAsync(new LoadProfileByNameSpec(name ?? string.Empty), ct)
            ?? throw new InvalidOperationException($"Unmapped load profile '{name}'.");

        return loadProfile.Id;
    }

    private async Task<int> GetReadCycleIdAsync(string? name, CancellationToken ct)
    {
        var readCycle = await readCycleRepository.FirstOrDefaultAsync(new ReadCycleByNameSpec(name ?? string.Empty), ct)
            ?? throw new InvalidOperationException($"Unmapped read cycle '{name}'.");

        return readCycle.Id;
    }

    private async Task<int> GetCountyIdAsync(string? name, CancellationToken ct)
    {
        var county = await countyRepository.FirstOrDefaultAsync(new CountyByNameSpec(name ?? string.Empty), ct)
            ?? throw new InvalidOperationException($"Unmapped county '{name}'.");

        return county.Id;
    }

    private static (string Zip, string? Zip4) SplitZip(string? postalCode)
    {
        var value = postalCode ?? string.Empty;

        return value.Length == 9
            ? (value[..5], value[5..])
            : (value, null);
    }
}
