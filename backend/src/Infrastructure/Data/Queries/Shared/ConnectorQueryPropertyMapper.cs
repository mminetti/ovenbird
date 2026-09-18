using Core.Shared;
using Infrastructure.Data.Queries.Common;

namespace Infrastructure.Data.Queries.Shared;

/// <summary>
/// Maps ConnectorDto property names to Connector entity property names for search and ordering.
/// </summary>
public class ConnectorQueryPropertyMapper : IQueryPropertyMapper<Connector>
{
    private static readonly Dictionary<string, string> _propertyMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["id"] = nameof(Connector.Id),
        ["name"] = nameof(Connector.Name),
        ["connectorTypeName"] = $"{nameof(Connector.ConnectorImplementation)}.{nameof(ConnectorImplementation.ConnectorType)}.{nameof(ConnectorType.Name)}",
        ["connectorImplementationName"] = $"{nameof(Connector.ConnectorImplementation)}.{nameof(ConnectorImplementation.Name)}"
    };

    private static readonly string[] _searchableProperties =
    [
        nameof(Connector.Name),
        $"{nameof(Connector.ConnectorImplementation)}.{nameof(ConnectorImplementation.ConnectorType)}.{nameof(ConnectorType.Name)}",
        $"{nameof(Connector.ConnectorImplementation)}.{nameof(ConnectorImplementation.Name)}"
    ];

    public string? MapToEntityProperty(string dtoPropertyName) =>
        _propertyMap.TryGetValue(dtoPropertyName, out var entityProperty) ? entityProperty : null;

    public string[] GetSearchableProperties() => _searchableProperties;
}
