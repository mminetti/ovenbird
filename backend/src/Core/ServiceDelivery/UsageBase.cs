using Core.Common;
using Core.Subscription;

namespace Core.ServiceDelivery;

public abstract class UsageBase : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;
    public long ServiceId { get; set; }
    public long ServicePointId { get; set; }
    public long? MeterId { get; set; }
    public DateOnly PeriodStartDate { get; set; }
    public DateOnly PeriodEndDate { get; set; }
    public double Consumption { get; set; }
    public int CommodityId { get; set; }
    public int UnitOfMeasureId { get; set; }

    public Service Service { get; set; } = default!;
    public ServicePoint ServicePoint { get; set; } = default!;
    public Meter? Meter { get; set; }
    public Commodity Commodity { get; set; } = default!;
    public UnitOfMeasure UnitOfMeasure { get; set; } = default!;
}
