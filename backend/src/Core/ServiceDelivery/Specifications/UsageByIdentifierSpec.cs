namespace Core.ServiceDelivery.Specifications;

public class UsageByIdentifierSpec : Specification<Usage>
{
    public UsageByIdentifierSpec(string identifier) =>
        Query.Where(usage => usage.Identifier == identifier);
}
