using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.Market.BigData.Builders;

public abstract class ServiceStartRejectImportBuilderBase(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : TransactionXmlReaderBase<ServiceRequestTransactionXml>, IMarketDocumentEntityBuilder
{
    protected override string RequiredElementName => "AccountMaint";

    public async Task CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transaction = DeserializeTransaction(item);
        var accountMaint = transaction.AccountMaint;

        var serviceStart = await GetServiceStartAsync(accountMaint.ProcessTrackingNumber, ct);

        foreach (var service in accountMaint.Services)
        {
            var rejectReason = service.ServiceRejectReasons.FirstOrDefault();

            var serviceStartItem = new ServiceStartItem
            {
                ServiceStartId = serviceStart.Id,
                Identifier = item.ReferenceNumber,
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

            serviceStart.ServiceOrderStatusId = Constants.ServiceOrderStatuses.Rejected;
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
