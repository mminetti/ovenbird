using Core.Common;

namespace Core.Shared;

public class Integration : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int IntegrationImplementationId { get; set; }

    public IntegrationImplementation IntegrationImplementation { get; set; } = default!;
    public ICollection<IntegrationField> IntegrationFields { get; set; } = [];
    public ICollection<Configuration> Configurations { get; set; } = [];

    public string? GetValue(string name)
    {
        return IntegrationFields
            .FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }

    public string GetRequiredValue(string name)
    {
        var value = GetValue(name);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Integration '{Name}' is missing required '{name}' field.");
        }

        return value;
    }
}
