namespace Core.ServiceDelivery.Specifications;

public class CountyByNameSpec : Specification<County>
{
    public CountyByNameSpec(string name) =>
        Query.Where(x => x.Name == name);
}
