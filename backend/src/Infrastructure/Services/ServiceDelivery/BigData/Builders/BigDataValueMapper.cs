using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

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

    private static readonly HashSet<string> _amsMeterAdvanceMeterInfoCodes = new(StringComparer.OrdinalIgnoreCase)
    {
        "AMSR",
    };

    public Result<int> GetCommodityId(string code) =>
        _commodityIdsByCode.TryGetValue(code, out var id)
            ? Result.Success(id)
            : Result.Invalid(new ValidationError(nameof(code), $"Unmapped commodity code '{code}'."));

    public Result<int> GetUnitOfMeasureId(string code) =>
        _unitOfMeasureIdsByCode.TryGetValue(code, out var id)
            ? Result.Success(id)
            : Result.Invalid(new ValidationError(nameof(code), $"Unmapped unit of measure code '{code}'."));

    public Result<int> GetUsageTypeId(string purposeCode) =>
        _usageTypeIdsByPurposeCode.TryGetValue(purposeCode, out var id)
            ? Result.Success(id)
            : Result.Invalid(new ValidationError(nameof(purposeCode), $"Unmapped usage purpose code '{purposeCode}'."));

    public bool IsAmsMeter(string? advanceMeterInfoCode) =>
        !string.IsNullOrWhiteSpace(advanceMeterInfoCode) && _amsMeterAdvanceMeterInfoCodes.Contains(advanceMeterInfoCode);
}
