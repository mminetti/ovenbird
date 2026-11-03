namespace Core.Processing;

public class ProcessRunLogItem : EntityBase<long>, IAggregateRoot
{
    public long ProcessRunId { get; set; }
    public int LevelId { get; set; }
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }

    public ProcessRun ProcessRun { get; set; } = default!;
    public ProcessRunLogLevel Level { get; set; } = default!;
}
