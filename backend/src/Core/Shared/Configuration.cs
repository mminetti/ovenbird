using Core.Common;

namespace Core.Shared;

public class Configuration : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ConfigurationTypeId { get; set; }
    public int? CompanyId { get; set; }

    public ICollection<ConfigurationField> ConfigurationFields { get; set; } = [];
    public ConfigurationType ConfigurationType { get; set; } = default!;
    public Company? Company { get; set; }

    public string? GetValue(string name)
    {
        return ConfigurationFields
            .FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }

    public string GetRequiredValue(string name)
    {
        var value = GetValue(name);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"Configuration '{Name}' is missing required '{name}' field.");
        }

        return value;
    }
}
