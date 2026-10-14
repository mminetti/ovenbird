using Infrastructure.Data.Queries.Common;

namespace Infrastructure.Data.Queries.Market;

/// <summary>
/// Maps MarketDto property names to Market entity property names for search and ordering.
/// </summary>
public class MarketQueryPropertyMapper : IQueryPropertyMapper<Core.Service.Market>
{
    private static readonly Dictionary<string, string> _propertyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = nameof(Core.Service.Market.Id),
        ["name"] = nameof(Core.Service.Market.Name),
        ["identifier"] = nameof(Core.Service.Market.Identifier)
    };

    private static readonly string[] _searchableProperties =
    [
        nameof(Core.Service.Market.Name),
        nameof(Core.Service.Market.Identifier)
    ];

    public string? MapToEntityProperty(string dtoPropertyName) =>
        _propertyMap.TryGetValue(dtoPropertyName, out var entityProperty) ? entityProperty : null;

    public string[] GetSearchableProperties() => _searchableProperties;
}
