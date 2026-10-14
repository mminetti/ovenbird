namespace Core.Service;

public class Utility : EntityBase<int>
{
    public string Identifier { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MarketId { get; set; }

    public Market Market { get; set; } = default!;
}
