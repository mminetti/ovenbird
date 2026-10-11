using System.Globalization;
using Core.Common.Constants;
using Core.Market;
using Core.Service;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.Market.BigData.Builders;

public class SwitchReject_814_02_Builder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : TransactionXmlReaderBase<ServiceRequestXml>, IMarketDocumentEntityBuilder
{
    public const string Key = "814_02";

    protected override string RequiredElementName => "AccountMaint";

    public async Task CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transaction = DeserializeTransaction(item);
        var accountMaint = transaction.AccountMaint;

        var serviceStart = await GetServiceStartAsync(accountMaint.ProcessTrackingNumber, ct);

        var serviceDate = DateOnly.ParseExact(accountMaint.DateStampReceived ?? string.Empty, DateFormat, CultureInfo.InvariantCulture);

        foreach (var service in accountMaint.Services)
        {
            var rejectReason = service.ServiceRejectReasons.FirstOrDefault();

            var serviceStartItem = new ServiceStartItem
            {
                ServiceStartId = serviceStart.Id,
                Identifier = item.ReferenceNumber,
                ServiceDate = serviceDate,
                ServiceActionId = Constants.ServiceActions.Reject,
                Description = $"{rejectReason?.RejectCode} - {rejectReason?.RejectReason}",
            };

            var created = await serviceStartItemRepository.AddAsync(serviceStartItem, ct);

            var reference = new MarketDocumentReference
            {
                MarketDocumentItemId = item.Id,
                ReferenceTypeId = Constants.MarketDocumentReferenceTypes.ServiceStart,
                ReferenceId = created.Id,
            };

            await referenceRepository.AddAsync(reference, ct);

            serviceStart.ServiceStatusId = Constants.ServiceStatuses.Rejected;
        }
    }

    // TODO: resolve ServiceStart
    private static Task<ServiceStart> GetServiceStartAsync(string? trackingNumber, CancellationToken ct) =>
        throw new NotImplementedException("ServiceStart resolution for 814 transactions is not yet implemented.");
}
