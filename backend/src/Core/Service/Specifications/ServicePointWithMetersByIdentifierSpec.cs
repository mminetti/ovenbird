namespace Core.Service.Specifications;

public class ServicePointWithMetersByIdentifierSpec : Specification<ServicePoint>
{
    public ServicePointWithMetersByIdentifierSpec(string identifier) =>
        Query
            .Include(x => x.Meters)
            .Where(x => x.Identifier == identifier);
}
