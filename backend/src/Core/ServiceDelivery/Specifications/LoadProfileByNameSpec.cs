namespace Core.ServiceDelivery.Specifications;

public class LoadProfileByNameSpec : Specification<LoadProfile>
{
    public LoadProfileByNameSpec(string name) =>
        Query.Where(x => x.Name == name);
}
