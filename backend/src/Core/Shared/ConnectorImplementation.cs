namespace Core.Shared;

public class ConnectorImplementation : EntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ConnectorTypeId { get; set; }

    public ConnectorType ConnectorType { get; set; } = default!;
    public ICollection<Connector> Connectors { get; set; } = [];
}
