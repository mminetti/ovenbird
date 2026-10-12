using Core.Common;

namespace Core.Subscription;

public class Customer : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
