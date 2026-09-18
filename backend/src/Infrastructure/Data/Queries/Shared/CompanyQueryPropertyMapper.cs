using Core.Market;
using Core.Shared;
using Infrastructure.Data.Queries.Common;

namespace Infrastructure.Data.Queries.Shared;

/// <summary>
/// Maps CompanyDto property names to Company entity property names for search and ordering.
/// </summary>
public class CompanyQueryPropertyMapper : IQueryPropertyMapper<Company>
{
    private static readonly Dictionary<string, string> _propertyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = nameof(Company.Id),
        ["name"] = nameof(Company.Name),
        ["marketName"] = $"{nameof(Company.Market)}.{nameof(Market.Name)}",
        ["timeZoneId"] = nameof(Company.TimeZoneId)
    };

    private static readonly string[] _searchableProperties =
    [
        nameof(Company.Name),
        $"{nameof(Company.Market)}.{nameof(Market.Name)}"
    ];

    public string? MapToEntityProperty(string dtoPropertyName) =>
        _propertyMap.TryGetValue(dtoPropertyName, out var entityProperty) ? entityProperty : null;

    public string[] GetSearchableProperties() => _searchableProperties;
}
