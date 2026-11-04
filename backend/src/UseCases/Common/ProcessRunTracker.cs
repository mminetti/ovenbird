using Core.Processing;

namespace UseCases.Common;

public class ProcessRunTracker(
    IRepository<ProcessRun> processRunRepository,
    IRepository<ProcessRunLogItem> logItemRepository,
    TimeProvider timeProvider)
{
    public Task<ProcessRun> StartAsync(int processTypeId, CancellationToken ct)
    {
        var processRun = new ProcessRun
        {
            ProcessTypeId = processTypeId,
            StatusId = Constants.ProcessRunStatuses.Running,
            StartedAtUtc = timeProvider.GetUtcNow(),
        };

        return processRunRepository.AddAsync(processRun, ct);
    }

    public async Task CompleteAsync(
        long processRunId, 
        bool succeeded, 
        IEnumerable<ValidationError> errors, 
        CancellationToken ct)
    {
        var logItems = errors.Select(e => ToLogItem(processRunId, e)).ToList();
        if (logItems.Count > 0)
        {
            await logItemRepository.AddRangeAsync(logItems, ct);
        }

        var run = await processRunRepository.GetByIdAsync(processRunId, ct)
            ?? throw new InvalidOperationException($"ProcessRun '{processRunId}' was not found.");

        run.StatusId = succeeded ? Constants.ProcessRunStatuses.Succeeded : Constants.ProcessRunStatuses.Failed;
        run.CompletedAtUtc = timeProvider.GetUtcNow();

        await processRunRepository.UpdateAsync(run, ct);
    }

    private static ProcessRunLogItem ToLogItem(long processRunId, ValidationError error)
    {
        var logItem = ErrorLogItemFactory.Create<ProcessRunLogItem>(error);
        logItem.ProcessRunId = processRunId;
        return logItem;
    }
}
