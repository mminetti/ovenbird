using Core.Common;

namespace Core.Service;

public class Premise : AuditableEntityBase<long>
{
    public string Address { get; set; } = string.Empty;
    public string Address2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string County { get; set; } = string.Empty;
}
