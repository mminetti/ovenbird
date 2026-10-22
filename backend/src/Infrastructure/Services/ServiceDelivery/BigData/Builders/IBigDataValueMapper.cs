namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public interface IBigDataValueMapper
{
    Result<int> GetCommodityId(string code);
    Result<int> GetUnitOfMeasureId(string code);
    Result<int> GetUsageTypeId(string purposeCode);
    bool IsAmsMeter(string? advanceMeterInfoCode);
}
