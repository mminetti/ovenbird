using Core.Common.Constants;

namespace Core.Service.Specifications;

public class ServiceStartItemsPendingExportSpec : Specification<ServiceStartItem>
{
    public ServiceStartItemsPendingExportSpec() =>
        Query
            .Include(x => x.ServiceStart)
                .ThenInclude(x => x.Account)
            .Where(x =>
                x.ServiceActionId == Constants.ServiceActions.Initiate &&
                x.ServiceStart.ServiceOrderStatusId == Constants.ServiceOrderStatuses.New);
}
