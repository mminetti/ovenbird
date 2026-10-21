namespace Core.ServiceDelivery.Specifications;

public class CustomerClassByNameSpec : Specification<CustomerClass>
{
    public CustomerClassByNameSpec(string name) =>
        Query.Where(x => x.Name == name);
}
