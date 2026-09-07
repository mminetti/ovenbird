using Core.Common;

namespace Core.Shared;

public class Connector : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ConnectorTypeId { get; set; }
    public int ConnectorImplementationId { get; set; }

    public ConnectorImplementation ConnectorImplementation { get; set; } = default!;
    public ConnectorType ConnectorType { get; set; } = default!;
    public ICollection<ConnectorField> ConnectorFields { get; set; } = [];
    public ICollection<Configuration> Configurations { get; set; } = [];

    public string? GetValue(string name)
    {
        return ConnectorFields
            .FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }

    public ConnectorField? GetField(string name)
    {
        return ConnectorFields
            .FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
    }
}
