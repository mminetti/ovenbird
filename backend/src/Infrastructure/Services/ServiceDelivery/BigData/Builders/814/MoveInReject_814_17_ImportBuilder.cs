using Core.MarketExchange;
using Core.ServiceDelivery;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class MoveInReject_814_17_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IRepository<MarketDocumentReference> referenceRepository)
    : ServiceStartRejectImportBuilderBase(serviceStartItemRepository, referenceRepository)
{
    public const string Key = $"BigData_{Set}_{SubSet}";
    private const string Set = "814";
    private const string SubSet = "17";
}
