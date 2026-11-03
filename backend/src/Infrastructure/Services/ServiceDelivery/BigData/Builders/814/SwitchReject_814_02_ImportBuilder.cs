using Core.ServiceDelivery;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class SwitchReject_814_02_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository)
    : ServiceStartRejectImportBuilderBase(serviceStartItemRepository)
{
    public const string Key = "BigData_814_02";
}
