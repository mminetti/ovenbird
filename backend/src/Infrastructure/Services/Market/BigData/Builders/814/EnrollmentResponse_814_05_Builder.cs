using System.Globalization;
using Core.Common.Constants;
using Core.Market;
using Core.Service;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.Market.BigData.Builders;

public class EnrollmentResponse_814_05_Builder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : TransactionXmlReaderBase<ServiceRequestXml>, IMarketDocumentEntityBuilder
{
    public const string Key = "814_05";

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

            var actionId = service.ActionCode == "A" ?
                    Constants.ServiceActions.Confirm :
                    Constants.ServiceActions.Reject;

            string? description = null;

            if (actionId == Constants.ServiceActions.Reject)
            {
                description = $"{service.RejectCode} - {service.RejectReason}";
            }

            var serviceStartItem = new ServiceStartItem
            {
                ServiceStartId = serviceStart.Id,
                Identifier = item.ReferenceNumber,
                ServiceDate = serviceDate,
                ServiceActionId = actionId,
                Description = description
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
            serviceStart.ServiceStatusId = actionId == Constants.ServiceActions.Confirm ?
                Constants.ServiceStatuses.Scheduled :
                Constants.ServiceStatuses.Rejected;
        }
    }

    // TODO: resolve ServiceStart 
    private static Task<ServiceStart> GetServiceStartAsync(string? trackingNumber, CancellationToken ct) =>
        throw new NotImplementedException("ServiceStart resolution for 814 transactions is not yet implemented.");
}
