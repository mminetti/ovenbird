using Core.Common;

namespace Core.Service;

public class ServicePoint : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;
}
