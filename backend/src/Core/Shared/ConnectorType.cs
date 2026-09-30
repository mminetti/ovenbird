using Core.Common;

namespace Core.Shared;

public class ConnectorType : EntityBase<int>, IHasDisplayName
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ICollection<ConnectorImplementation> ConnectorImplementations { get; set; } = [];
}
