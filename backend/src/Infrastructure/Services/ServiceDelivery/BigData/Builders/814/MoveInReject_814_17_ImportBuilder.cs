using Core.ServiceDelivery;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class MoveInReject_814_17_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository)
    : ServiceStartRejectImportBuilderBase(serviceStartItemRepository)
{
    public const string Key = $"BigData_{Set}_{SubSet}";
    private const string Set = "814";
    private const string SubSet = "17";
}
