using Core.Common;

namespace Core.Processing;

public class ProcessRunLogItem : ErrorLogItem
{
    public long ProcessRunId { get; set; }

    public ProcessRun ProcessRun { get; set; } = default!;
}
