using Infrastructure.Data.Queries.Common;

namespace Infrastructure.Data.Queries.Markets;

/// <summary>
/// Maps MarketDto property names to Market entity property names for search and ordering.
/// </summary>
public class MarketQueryPropertyMapper : IQueryPropertyMapper<Core.ServiceDelivery.Market>
{
    private static readonly Dictionary<string, string> _propertyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = nameof(Core.ServiceDelivery.Market.Id),
        ["name"] = nameof(Core.ServiceDelivery.Market.Name),
        ["identifier"] = nameof(Core.ServiceDelivery.Market.Identifier)
    };

    private static readonly string[] _searchableProperties =
    [
        nameof(Core.ServiceDelivery.Market.Name),
        nameof(Core.ServiceDelivery.Market.Identifier)
    ];

    public string? MapToEntityProperty(string dtoPropertyName) =>
        _propertyMap.TryGetValue(dtoPropertyName, out var entityProperty) ? entityProperty : null;

    public string[] GetSearchableProperties() => _searchableProperties;
}
