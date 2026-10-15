using Core.MarketExchange;
using Core.ServiceDelivery;

namespace Infrastructure.Services.Market.BigData.Builders;

public class MoveInReject_814_17_Builder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : ServiceStartRejectBuilderBase(serviceStartItemRepository, referenceRepository)
{
    public const string Key = "814_17";
}
