using Core.Shared;

namespace Core.Market;

public class Market : EntityBase<int>
{
    public string Name { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
    public string TimeZoneId { get; set; } = string.Empty;

    public ICollection<Company> Companies { get; set; } = [];
}
