using Core.Common.Constants;

namespace Core.Service.Specifications;

public class ServiceStartItemsPendingExportSpec : Specification<ServiceStartItem>
{
    public ServiceStartItemsPendingExportSpec(int companyId) =>
        Query
            .Include(x => x.ServiceStart)
                .ThenInclude(x => x.Account)
                    .ThenInclude(x => x.Customer)
            .Include(x => x.ServiceStart)
                .ThenInclude(x => x.Account)
                    .ThenInclude(x => x.Service)
                        .ThenInclude(x => x.ServicePoint)
                            .ThenInclude(x => x.Premise)
            .Include(x => x.ServiceStart)
                .ThenInclude(x => x.Account)
                    .ThenInclude(x => x.Service)
                        .ThenInclude(x => x.ServicePoint)
                            .ThenInclude(x => x.Utility)
            .Where(x =>
                x.ServiceActionId == Constants.ServiceActions.Initiate &&
                x.ServiceStart.ServiceOrderStatusId == Constants.ServiceOrderStatuses.New &&
                x.ServiceStart.Account.CompanyId == companyId);
}
