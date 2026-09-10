using Core.Security;
using Infrastructure.Data.Queries.Common;

namespace Infrastructure.Data.Queries.Security;

/// <summary>
/// Maps UserDto property names to User entity property names for search and ordering.
/// </summary>
public class UserQueryPropertyMapper : IQueryPropertyMapper<User>
{
    private static readonly Dictionary<string, string> _propertyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = nameof(User.Id),
        ["name"] = nameof(User.Name),
        ["email"] = nameof(User.Email),
        ["isactive"] = nameof(User.IsActive)
    };

    private static readonly string[] _searchableProperties =
    [
        nameof(User.Name),
        nameof(User.Email)
    ];

    public string? MapToEntityProperty(string dtoPropertyName) =>
        _propertyMap.TryGetValue(dtoPropertyName, out var entityProperty) ? entityProperty : null;

    public string[] GetSearchableProperties() => _searchableProperties;
}
