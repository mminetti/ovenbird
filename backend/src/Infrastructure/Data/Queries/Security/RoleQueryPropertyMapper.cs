using Core.Security;
using Infrastructure.Data.Queries.Common;

namespace Infrastructure.Data.Queries.Security;

/// <summary>
/// Maps RoleDto property names to Role entity property names for search and ordering.
/// </summary>
public class RoleQueryPropertyMapper : IQueryPropertyMapper<Role>
{
    private static readonly Dictionary<string, string> _propertyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = nameof(Role.Id),
        ["name"] = nameof(Role.Name)
    };

    private static readonly string[] _searchableProperties =
    [
        nameof(Role.Name)
    ];

    public string? MapToEntityProperty(string dtoPropertyName) =>
        _propertyMap.TryGetValue(dtoPropertyName, out var entityProperty) ? entityProperty : null;

    public string[] GetSearchableProperties() => _searchableProperties;
}
