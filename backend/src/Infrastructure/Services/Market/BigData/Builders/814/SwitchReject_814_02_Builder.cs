using Core.MarketExchange;
using Core.ServiceDelivery;

namespace Infrastructure.Services.Market.BigData.Builders;

public class SwitchReject_814_02_Builder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : ServiceStartRejectBuilderBase(serviceStartItemRepository, referenceRepository)
{
    public const string Key = "BigData_814_02";
}
