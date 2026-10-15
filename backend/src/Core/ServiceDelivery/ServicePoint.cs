using Core.Common;

namespace Core.ServiceDelivery;

public class ServicePoint : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;
    public long PremiseId { get; set; }
    public int UtilityId { get; set; }
    public int ReadCycleId { get; set; }
    public bool IsCriticalCare { get; set; }
    public int CustomerClassId { get; set; }
    public int LoadProfileId { get; set; }
    public bool IsAmsMeter { get; set; }
    public int CommodityId { get; set; }
    public int MarketId { get; set; }

    public Premise Premise { get; set; } = default!;
    public Utility Utility { get; set; } = default!;
    public ReadCycle ReadCycle { get; set; } = default!;
    public CustomerClass CustomerClass { get; set; } = default!;
    public LoadProfile LoadProfile { get; set; } = default!;
    public Commodity Commodity { get; set; } = default!;
    public Market Market { get; set; } = default!;

    public ICollection<Meter> Meters { get; set; } = [];

    public Meter? GetMeter(string identifier)
    {
        return Meters?
            .FirstOrDefault(x => string.Equals(x.Identifier, identifier, StringComparison.OrdinalIgnoreCase));
    }
}
