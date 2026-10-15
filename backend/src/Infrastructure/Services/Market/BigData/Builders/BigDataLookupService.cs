using Core.ServiceDelivery;
using Core.ServiceDelivery.Specifications;
using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.Market.BigData.Builders;

public class BigDataLookupService(
    IReadRepository<ServicePoint> servicePointReadRepository)
    : IBigDataLookupService
{
    private static readonly Dictionary<string, int> _commodityIdsByCode = new()
    {
        ["E"] = CoreConstants.Commodities.Electricity,
    };

    private static readonly Dictionary<string, int> _unitOfMeasureIdsByCode = new()
    {
        ["KH"] = CoreConstants.UnitOfMeasures.KilowattHour,
    };

    private static readonly Dictionary<string, int> _usageTypeIdsByPurposeCode = new()
    {
        ["00"] = CoreConstants.UsageTypes.Original,
        ["01"] = CoreConstants.UsageTypes.Cancel,
        ["05"] = CoreConstants.UsageTypes.Replace,
    };

    public int GetCommodityId(string code) =>
        _commodityIdsByCode.TryGetValue(code, out var id)
            ? id
            : throw new InvalidOperationException($"Unmapped commodity code '{code}'.");

    public int GetUnitOfMeasureId(string code) =>
        _unitOfMeasureIdsByCode.TryGetValue(code, out var id)
            ? id
            : throw new InvalidOperationException($"Unmapped unit of measure code '{code}'.");

    public int GetUsageTypeId(string purposeCode) =>
        _usageTypeIdsByPurposeCode.TryGetValue(purposeCode, out var id)
            ? id
            : throw new InvalidOperationException($"Unmapped usage purpose code '{purposeCode}'.");

    public async Task<ServicePoint> GetServicePointAsync(string identifier, CancellationToken ct)
    {
        return await servicePointReadRepository.FirstOrDefaultAsync(
            new ServicePointWithMetersByIdentifierSpec(identifier), ct)
            ?? throw new InvalidOperationException($"Service Point '{identifier}' was not found.");
    }
}
