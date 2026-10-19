using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.Market.BigData.Builders;

public class BigDataValueMapper : IBigDataValueMapper
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
        ["00"] = CoreConstants.UsagePurposes.Original,
        ["01"] = CoreConstants.UsagePurposes.Cancel,
        ["05"] = CoreConstants.UsagePurposes.Replace,
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
}
