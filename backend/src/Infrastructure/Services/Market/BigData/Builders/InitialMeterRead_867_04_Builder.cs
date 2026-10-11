using System.Globalization;
using Core.Market;
using Core.Service;
using Core.Service.Specifications;
using Core.Subscription;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.Market.BigData.Builders;

public class InitialMeterRead_867_04_Builder(
    IReadRepository<Account> accountReadRepository,
    IReadRepository<ServiceStart> serviceStartReadRepository,
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : UsageTransactionReaderBase(accountReadRepository), IMarketDocumentEntityBuilder
{
    public const string Key = "867_04";

    private const string InitialUsageType = "INT";

    public async Task CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transaction = DeserializeTransaction(item);
        var usage = transaction.Usage;

        var account = await GetAccountAsync(usage.TranNr814, ct);
        var serviceStart = await GetServiceStartAsync(account, ct);

        foreach (var usageMeter in usage.UsageMeters.Where(x => x.UsageType == InitialUsageType))
        {
            var serviceDate = DateOnly.ParseExact(usageMeter.SwitchDate ?? string.Empty, DateFormat, CultureInfo.InvariantCulture);

            foreach (var quantity in usageMeter.UsageQuantities)
            {
                foreach (var read in quantity.UsageReads)
                {
                    var serviceStartItem = new ServiceStartItem
                    {
                        ServiceStartId = serviceStart.Id,
                        Identifier = item.ReferenceNumber,
                        ServiceDate = serviceDate,
                        ServiceActionId = CoreConstants.ServiceActions.Complete,
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
        }
    }

    private async Task<ServiceStart> GetServiceStartAsync(Account account, CancellationToken ct)
    {
        // TODO: figure out the correct matching logic for selecting the right ServiceStart once an account can have more than one.

        return await serviceStartReadRepository.FirstOrDefaultAsync(new ServiceStartByAccountIdSpec(account.Id), ct)
            ?? throw new InvalidOperationException($"ServiceStart for account '{account.Identifier}' was not found.");
    }
}
