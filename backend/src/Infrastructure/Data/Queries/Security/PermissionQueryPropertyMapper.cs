using Core.Security;
using Infrastructure.Data.Queries.Common;

namespace Infrastructure.Data.Queries.Security;

/// <summary>
/// Maps PermissionDto property names to Permission entity property names for search and ordering.
/// </summary>
public class PermissionQueryPropertyMapper : IQueryPropertyMapper<Permission>
{
    private static readonly Dictionary<string, string> _propertyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = nameof(Permission.Id),
        ["name"] = nameof(Permission.Name),
        ["description"] = nameof(Permission.Description)
    };

    private static readonly string[] _searchableProperties =
    [
        nameof(Permission.Name),
        nameof(Permission.Description)
    ];

    public string? MapToEntityProperty(string dtoPropertyName) =>
        _propertyMap.TryGetValue(dtoPropertyName, out var entityProperty) ? entityProperty : null;

    public string[] GetSearchableProperties() => _searchableProperties;
}
