using Core.Common.Constants;
using Core.MarketExchange;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;

namespace Infrastructure.Services.Market.BigData.Builders;

public class Usage_867_03_Builder(
    IReadRepository<Usage> usageReadRepository,
    IBigDataLookupService lookupService,
    IRepository<Usage> usageRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : UsageTransactionBuilderBase<Usage>(lookupService, usageRepository, referenceRepository)
{
    public const string Key = "867_03";

    private const string DetailUsageType = "DET";

    private int _usageTypeId;
    private bool _isFinal;
    private long? _originalUsageId;

    protected override int ReferenceTypeId => Constants.MarketDocumentReferenceTypes.Usage;

    protected override IEnumerable<UsageMeterXml> SelectUsageMeters(IEnumerable<UsageMeterXml> usageMeters) =>
        usageMeters.Where(x => x.UsageType == DetailUsageType);

    protected override async Task PrepareAsync(UsageXml usage, CancellationToken ct)
    {
        _usageTypeId = LookupService.GetUsageTypeId(usage.PurposeCode ?? string.Empty);
        _isFinal = usage.FinalIndicator == "F";
        _originalUsageId = await GetOriginalUsageIdAsync(usage.OriginalTransactionNumber, ct);
    }

    protected override Usage CreateUsage(UsageReadXml read) => new()
    {
        UsageTypeId = _usageTypeId,
        OriginalUsageId = _originalUsageId,
        IsFinal = _isFinal,
        StartRead = read.StartMeterRead ?? 0,
        EndRead = read.EndMeterRead ?? 0,
        UsageStatusId = Constants.UsageStatuses.New,
    };

    private async Task<long?> GetOriginalUsageIdAsync(string? originalTransactionNumber, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(originalTransactionNumber))
        {
            return null;
        }

        var originalUsage = await usageReadRepository.FirstOrDefaultAsync(new UsageByIdentifierSpec(originalTransactionNumber), ct)
            ?? throw new InvalidOperationException($"Original usage '{originalTransactionNumber}' was not found.");

        return originalUsage.Id;
    }
}
