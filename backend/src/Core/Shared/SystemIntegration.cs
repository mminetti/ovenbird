using Core.Common;

namespace Core.Shared;

public class SystemIntegration : AuditableEntityBase<int>
{
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CompanyId { get; set; }

    public IList<SystemIntegrationField> SystemIntegrationFields { get; set; } = [];
    public Company? Company { get; set; }

    public string? GetValue(string identifier)
    {
        return SystemIntegrationFields
            .FirstOrDefault(x => string.Equals(x.Identifier, identifier, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }

    public string GetRequiredValue(string identifier)
    {
        var value = GetValue(identifier);

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"System Integration '{Name}' is missing required '{identifier}' field.");
        }

        return value;
    }
}
