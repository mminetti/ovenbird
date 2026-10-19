using System.Globalization;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.Market.BigData.Builders;

public class EnrollmentNotification_814_05_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
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

            serviceStart.ServiceDate = serviceDate;
            serviceStart.ServiceOrderStatusId = Constants.ServiceOrderStatuses.Scheduled;
        }
    }

    // TODO: resolve ServiceStart 
    private static Task<ServiceStart> GetServiceStartAsync(string? trackingNumber, CancellationToken ct) =>
        throw new NotImplementedException("ServiceStart resolution for 814 transactions is not yet implemented.");
}
