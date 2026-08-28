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

    public string GetRequiredValue(string name)
    {
        var value = GetValue(name);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Connector '{Name}' is missing required '{name}' field.");
        }

        return value;
    }
}
