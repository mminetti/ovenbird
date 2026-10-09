namespace Core.Service.Specifications;

public class ServicePointByIdentifierSpec : Specification<ServicePoint>
{
    public ServicePointByIdentifierSpec(string identifier) =>
        Query.Where(servicePoint => servicePoint.Identifier == identifier);
}
