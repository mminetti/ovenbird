namespace Core.ServiceDelivery.Specifications;

public class ReadCycleByNameSpec : Specification<ReadCycle>
{
    public ReadCycleByNameSpec(string name) =>
        Query.Where(x => x.Name == name);
}
