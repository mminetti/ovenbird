using System.Globalization;
using Core.Market;
using Core.Service;
using UseCases.Market.MarketDocuments.Import.Interfaces;
using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.Market.BigData.Builders;

public class InitialMeterRead_867_04_Builder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : TransactionXmlReaderBase<UsageTransactionXml>, IMarketDocumentEntityBuilder
{
    public const string Key = "867_04";

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

    // TODO: resolve ServiceStart 
    private static Task<ServiceStart> GetServiceStartAsync(string? sourceReferenceNumber, CancellationToken ct) =>
        throw new NotImplementedException("ServiceStart resolution for 867_04 transactions is not yet implemented.");
}
