using Core.Market;
using Infrastructure.Data.Queries.Common;

namespace Infrastructure.Data.Queries.Shared;

/// <summary>
/// Maps MarketDto property names to Market entity property names for search and ordering.
/// </summary>
public class MarketQueryPropertyMapper : IQueryPropertyMapper<Market>
{
    private static readonly Dictionary<string, string> _propertyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = nameof(Market.Id),
        ["name"] = nameof(Market.Name),
        ["identifier"] = nameof(Market.Identifier)
    };

    private static readonly string[] _searchableProperties =
    [
        nameof(Market.Name),
        nameof(Market.Identifier)
    ];

    public string? MapToEntityProperty(string dtoPropertyName) =>
        _propertyMap.TryGetValue(dtoPropertyName, out var entityProperty) ? entityProperty : null;

    public string[] GetSearchableProperties() => _searchableProperties;
}
