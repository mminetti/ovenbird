using Core.Common;

namespace Core.Shared;

public class Company : AuditableEntityBase<int>
{
    public string Name { get; set; } = string.Empty;
}
