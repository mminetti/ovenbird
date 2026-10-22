namespace Core.Processing;

public class ProcessRun : EntityBase<long>, IAggregateRoot
{
    public int ProcessTypeId { get; set; }
    public int StatusId { get; set; }
    public DateTimeOffset StartedAtUtc { get; set; }
    public DateTimeOffset? CompletedAtUtc { get; set; }

    public ProcessType ProcessType { get; set; } = default!;
    public ProcessRunStatus Status { get; set; } = default!;
    public ICollection<ProcessRunLogItem> LogItems { get; set; } = [];
}
