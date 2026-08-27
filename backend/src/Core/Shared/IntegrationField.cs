using Core.Common;

namespace Core.Shared;

public class IntegrationField : AuditableEntityBase<int>
{
    public int IntegrationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }
    public bool IsSecret { get; set; }

    public Integration Integration { get; set; } = default!;
}
