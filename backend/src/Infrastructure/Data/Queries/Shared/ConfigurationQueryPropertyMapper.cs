using Core.Shared;
using Infrastructure.Data.Queries.Common;

namespace Infrastructure.Data.Queries.Shared;

/// <summary>
/// Maps ConfigurationDto property names to Configuration entity property names for search and ordering.
/// </summary>
public class ConfigurationQueryPropertyMapper : IQueryPropertyMapper<Configuration>
{
    private static readonly Dictionary<string, string> _propertyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = nameof(Configuration.Id),
        ["name"] = nameof(Configuration.Name),
        ["configurationTypeName"] = $"{nameof(Configuration.ConfigurationType)}.{nameof(ConfigurationType.Name)}",
        ["companyName"] = $"{nameof(Configuration.Company)}.{nameof(Company.Name)}"
    };

    private static readonly string[] _searchableProperties =
    [
        nameof(Configuration.Name),
        $"{nameof(Configuration.ConfigurationType)}.{nameof(ConfigurationType.Name)}"
    ];

    public string? MapToEntityProperty(string dtoPropertyName) =>
        _propertyMap.TryGetValue(dtoPropertyName, out var entityProperty) ? entityProperty : null;

    public string[] GetSearchableProperties() => _searchableProperties;
}
