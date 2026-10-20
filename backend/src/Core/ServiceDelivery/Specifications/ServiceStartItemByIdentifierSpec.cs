namespace Core.ServiceDelivery.Specifications;

public class ServiceStartItemByIdentifierSpec : Specification<ServiceStartItem>
{
    public ServiceStartItemByIdentifierSpec(string identifier) =>
        Query
            .Include(x => x.ServiceStart)
                .ThenInclude(x => x.Service)
                    .ThenInclude(x => x.ServicePoint)
                        .ThenInclude(x => x.Meters)
            .Where(x => x.Identifier == identifier);
}
