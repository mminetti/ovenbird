using Core.MarketExchange;
using Core.ServiceDelivery;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class SwitchReject_814_02_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : ServiceStartRejectImportBuilderBase(serviceStartItemRepository, referenceRepository)
{
    public const string Key = "BigData_814_02";
}
