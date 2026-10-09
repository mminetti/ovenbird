using Core.Common;

namespace Core.Service;

public class ServicePoint : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;

    public ICollection<Meter> Meters { get; set; } = [];

    public Meter? GetMeter(string identifier)
    {
        return Meters?
            .FirstOrDefault(x => string.Equals(x.Identifier, identifier, StringComparison.OrdinalIgnoreCase));
    }
}
