using Core.Common;

namespace Core.Shared;

public class SystemIntegrationField : AuditableEntityBase<int>
{
    public int SystemIntegrationId { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }

    public SystemIntegration SystemIntegration { get; set; } = default!;
}
