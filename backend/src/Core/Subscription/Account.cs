using Core.Common;

namespace Core.Subscription;

public class Account : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;
}
