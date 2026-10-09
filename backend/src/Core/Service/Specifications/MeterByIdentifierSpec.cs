namespace Core.Service.Specifications;

public class MeterByIdentifierSpec : Specification<Meter>
{
    public MeterByIdentifierSpec(string identifier) =>
        Query.Where(meter => meter.Identifier == identifier);
}
