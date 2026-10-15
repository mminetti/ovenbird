namespace Core.ServiceDelivery.Specifications;

public class MeterByIdentifierSpec : Specification<Meter>
{
    public MeterByIdentifierSpec(string identifier) =>
        Query.Where(meter => meter.Identifier == identifier);
}
