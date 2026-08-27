using Core.Common;

namespace Core.Shared;

public class ConfigurationField : AuditableEntityBase<int>
{
    public int ConfigurationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }

    public Configuration Configuration { get; set; } = default!;
    public ICollection<Integration> Integrations { get; set; } = [];
}
