using Core.Common;

namespace Core.Service;

public class ServicePoint : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;
    public long PremiseId { get; set; }
    public int UtilityId { get; set; }
    public string Cycle { get; set; } = string.Empty;
    public bool IsCriticalCare { get; set; }
    public int PremiseTypeId { get; set; }
    public int CommodityId { get; set; }
    public int MarketId { get; set; }

    public Premise Premise { get; set; } = default!;
    public Utility Utility { get; set; } = default!;
    public PremiseType PremiseType { get; set; } = default!;
    public Commodity Commodity { get; set; } = default!;
    public Market.Market Market { get; set; } = default!;

    public ICollection<Meter> Meters { get; set; } = [];

    public Meter? GetMeter(string identifier)
    {
        return Meters?
            .FirstOrDefault(x => string.Equals(x.Identifier, identifier, StringComparison.OrdinalIgnoreCase));
    }
}
