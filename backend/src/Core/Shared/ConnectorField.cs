using Core.Common;

namespace Core.Shared;

public class ConnectorField : AuditableEntityBase<int>
{
    public int ConnectorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }
    public bool IsSecret { get; set; }

    public Connector Connector { get; set; } = default!;
}
