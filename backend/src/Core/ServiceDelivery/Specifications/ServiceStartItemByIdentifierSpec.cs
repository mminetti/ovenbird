namespace Core.ServiceDelivery.Specifications;

public class ServiceStartItemByIdentifierSpec : Specification<ServiceStartItem>
{
    public ServiceStartItemByIdentifierSpec(string identifier) =>
        Query
            .Include(x => x.ServiceStart)
            .Where(x => x.Identifier == identifier);
}
