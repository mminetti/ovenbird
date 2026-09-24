using Core.Common;
using Core.Shared;

namespace Core.Market;

public class Market : AuditableEntityBase<int>, IAudited
{
    public string Name { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;

    public ICollection<Company> Companies { get; set; } = [];
}
