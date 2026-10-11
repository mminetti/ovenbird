using System.Globalization;
using Core.Market;
using Core.Service;
using Core.Service.Specifications;
using Core.Subscription;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.Market.BigData.Builders;

public class EnrollmentResponse_814_05_Builder(
    IReadRepository<ServiceStart> serviceStartReadRepository,
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : ServiceRequestReaderBase, IMarketDocumentEntityBuilder
{
    public const string Key = "814_05";

    public async Task CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transaction = DeserializeTransaction(item);
        var accountMaint = transaction.AccountMaint;

        var account = await GetAccountAsync(accountMaint, ct);
        var serviceStart = await GetServiceStartAsync(account, ct);

        foreach (var service in accountMaint.Services)
        {
            var customerAgreementDate = service.ServiceDates
                .Select(x => x.CustomerAgreementDate)
                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x))
                ?? throw new InvalidOperationException($"MarketDocumentItem '{item.Id}' has no CustomerAgreementDate.");

            var actionId = service.ActionCode == "A" ? 
                    CoreConstants.ServiceActions.Confirm : 
                    CoreConstants.ServiceActions.Reject;

            string? description = null;

            if (actionId == CoreConstants.ServiceActions.Reject)
            {
                description = $"{service.RejectCode} - {service.RejectReason}"; 
            }

            var serviceStartItem = new ServiceStartItem
            {
                ServiceStartId = serviceStart.Id,
                Identifier = item.ReferenceNumber,
                ServiceDate = DateOnly.ParseExact(customerAgreementDate, DateFormat, CultureInfo.InvariantCulture),
                ServiceActionId = actionId,
                Description = description
            };

            var created = await serviceStartItemRepository.AddAsync(serviceStartItem, ct);

            var reference = new MarketDocumentReference
            {
                MarketDocumentItemId = item.Id,
                ReferenceTypeId = CoreConstants.MarketDocumentReferenceTypes.ServiceStart,
                ReferenceId = created.Id,
            };

            await referenceRepository.AddAsync(reference, ct);
        }
    }

    private async Task<ServiceStart> GetServiceStartAsync(Account account, CancellationToken ct)
    {
        // TODO: figure out the correct matching logic for selecting the right ServiceStart once an account can have more than one.

        return await serviceStartReadRepository.FirstOrDefaultAsync(new ServiceStartByAccountIdSpec(account.Id), ct)
            ?? throw new InvalidOperationException($"ServiceStart for account '{account.Identifier}' was not found.");
    }
}
