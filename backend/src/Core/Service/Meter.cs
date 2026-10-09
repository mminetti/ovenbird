using Core.Common;

namespace Core.Service;

public class Meter : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;
}
