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
    IMessageBus bus)
    : TransactionXmlReaderBase<ServiceRequestTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = "BigData_814_05";
    protected override string RequiredElementName => "AccountMaint";

    public async Task CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transaction = DeserializeTransaction(item);
        var accountMaint = transaction.AccountMaint;

        var serviceStart = await GetServiceStartAsync(accountMaint.ProcessTrackingNumber, ct);

        foreach (var service in accountMaint.Services)
        {
            var customerAgreementDate = service.ServiceDates
                .Select(x => x.CustomerAgreementDate)
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))
                ?? throw new InvalidOperationException($"MarketDocumentItem '{item.Id}' has no CustomerAgreementDate.");

            var serviceDate = DateOnly.ParseExact(customerAgreementDate, DateFormat, CultureInfo.InvariantCulture);

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

            await bus.PublishAsync(new ServiceStartScheduledEvent(serviceStart, serviceDate, item.Id));
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
            ?? throw new InvalidOperationException($"ServiceStartItem with tracking number '{trackingNumber}' was not found.");

        return serviceStartItem.ServiceStart;
    }
}
