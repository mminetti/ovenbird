using System.Globalization;
using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using UseCases.Market.MarketDocuments.Import.Interfaces;

namespace Infrastructure.Services.Market.BigData.Builders;

public class InitialMeterRead_867_04_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : TransactionXmlReaderBase<UsageTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = "BigData_867_04";
    private const string InitialUsageType = "INT";
    protected override string RequiredElementName => "Usage";

    public async Task CreateAsync(MarketDocumentItem item, CancellationToken ct)
    {
        var transaction = DeserializeTransaction(item);
        var usage = transaction.Usage;

        var serviceStart = await GetServiceStartAsync(usage.TranNr814, ct);

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
                        ServiceActionId = Constants.ServiceActions.Complete,
                    };

                    var created = await serviceStartItemRepository.AddAsync(serviceStartItem, ct);

                    var reference = new MarketDocumentReference
                    {
                        MarketDocumentItemId = item.Id,
                        ReferenceTypeId = Constants.MarketDocumentReferenceTypes.ServiceStart,
                        ReferenceId = created.Id,
                    };

                    await referenceRepository.AddAsync(reference, ct);
                }
            }

            // TODO: move this to an event handler
            serviceStart.ServiceDate = serviceDate;
            serviceStart.ServiceOrderStatusId = Constants.ServiceOrderStatuses.Completed;
        }
    }

    private async Task<ServiceStart> GetServiceStartAsync(string? sourceReferenceNumber, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(sourceReferenceNumber))
        {
            throw new InvalidOperationException("Transaction has no TranNr814.");
        }

        var serviceStartItem = await serviceStartItemRepository.FirstOrDefaultAsync(
            new ServiceStartItemByIdentifierSpec(sourceReferenceNumber), ct)
            ?? throw new InvalidOperationException($"ServiceStartItem with tracking number '{sourceReferenceNumber}' was not found.");

        return serviceStartItem.ServiceStart;
    }
}
