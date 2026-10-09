using Core.Common;
using Core.Subscription;

namespace Core.Service;

public class HistoricalUsage : AuditableEntityBase<long>
{
    public string Identifier { get; set; } = string.Empty;
    public long AccountId { get; set; }
    public long ServicePointId { get; set; }
    public long? MeterId { get; set; }
    public DateOnly PeriodStartDate { get; set; }
    public DateOnly PeriodEndDate { get; set; }
    public double Consumption { get; set; }
    public int CommodityId { get; set; }
    public int UnitOfMeasureId { get; set; }

    public Account Account { get; set; } = default!;
    public ServicePoint ServicePoint { get; set; } = default!;
    public Meter? Meter { get; set; }
    public Commodity Commodity { get; set; } = default!;
    public UnitOfMeasure UnitOfMeasure { get; set; } = default!;
}
