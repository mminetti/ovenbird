using CoreConstants = Core.Common.Constants.Constants;

namespace Infrastructure.Services.Market.BigData.EntityBuilders;

public static class EdiCodeMappings
{
    private static readonly Dictionary<string, int> CommodityIdsByCode = new()
    {
        ["E"] = CoreConstants.Commodities.Electricity,
    };

    private static readonly Dictionary<string, int> UnitOfMeasureIdsByCode = new()
    {
        ["KH"] = CoreConstants.UnitOfMeasures.KilowattHour,
    };

    public static int ResolveCommodityId(string code) =>
        CommodityIdsByCode.TryGetValue(code, out var id)
            ? id
            : throw new InvalidOperationException($"Unmapped commodity code '{code}'.");

    public static int ResolveUnitOfMeasureId(string code) =>
        UnitOfMeasureIdsByCode.TryGetValue(code, out var id)
            ? id
            : throw new InvalidOperationException($"Unmapped unit of measure code '{code}'.");
}
