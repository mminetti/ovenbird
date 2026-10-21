namespace Infrastructure.Services.Market.BigData.Builders;

public interface IBigDataValueMapper
{
    int GetCommodityId(string code);
    int GetUnitOfMeasureId(string code);
    int GetUsageTypeId(string purposeCode);
    bool IsAmsMeter(string? advanceMeterInfoCode);
}
