namespace Core.ServiceDelivery;

public class Usage : UsageBase
{
    public int UsagePurposeId { get; set; } // original, cancel, replace
    public int UsageTypeId { get; set; } // non interval, interval
    public long? OriginalUsageId { get; set; }
    public bool IsFinal { get; set; }
    public int StartRead { get; set; }
    public int EndRead { get; set; }
    public int UsageStatusId { get; set; }

    public UsagePurpose UsagePurpose { get; set; } = default!;
    public UsageType UsageType { get; set; } = default!;
    public Usage? OriginalUsage { get; set; }
    public UsageStatus UsageStatus { get; set; } = default!;
}
