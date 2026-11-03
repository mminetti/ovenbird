using Core.ServiceDelivery;
using UseCases.Interfaces.ServiceDelivery;

namespace Infrastructure.Services.ServiceDelivery.BigData.Builders;

public class SwitchReject_814_02_ImportBuilder(
    IRepository<ServiceStartItem> serviceStartItemRepository,
    IServiceStartService serviceStartService)
    : ServiceStartRejectImportBuilderBase(serviceStartItemRepository, serviceStartService)
{
    public const string Key = "BigData_814_02";
}
