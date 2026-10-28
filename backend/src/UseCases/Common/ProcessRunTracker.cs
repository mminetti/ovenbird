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

    private ProcessRunLogItem ToLogItem(long processRunId, ValidationError error)
    {
        _ = EntityReference.TryParse(error.Identifier, out var entityType, out var entityId);

        return new ProcessRunLogItem
        {
            ProcessRunId = processRunId,
            LevelId = MapSeverity(error.Severity),
            EntityType = entityType,
            EntityId = entityId,
            Field = error.ErrorCode,
            Message = error.ErrorMessage,
            CreatedAtUtc = timeProvider.GetUtcNow(),
        };
    }

    private static int MapSeverity(ValidationSeverity severity) => severity switch
    {
        ValidationSeverity.Warning => Constants.ProcessRunLogLevels.Warning,
        ValidationSeverity.Info => Constants.ProcessRunLogLevels.Info,
        _ => Constants.ProcessRunLogLevels.Error,
    };
}
