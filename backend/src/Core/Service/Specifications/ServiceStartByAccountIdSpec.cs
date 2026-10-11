namespace Core.Service.Specifications;

public class ServiceStartByAccountIdSpec : Specification<ServiceStart>
{
    public ServiceStartByAccountIdSpec(long accountId) =>
        Query.Where(serviceStart => serviceStart.AccountId == accountId);
}
