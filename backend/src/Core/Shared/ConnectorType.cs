namespace Core.Shared;

public class ConnectorType : EntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<Connector> Connectors { get; set; } = [];
}
