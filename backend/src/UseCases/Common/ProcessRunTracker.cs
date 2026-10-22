using Core.Processing;
using CoreConstants = Core.Common.Constants.Constants;

namespace UseCases.Common;

/// <summary>
/// Records one execution of a recurring process (an import/export job run) and the log items
/// raised during it, so failures can be traced back to the run - and, via <see cref="EntityReference"/>,
/// back to the specific record that had the problem - independently of Quartz's own scheduler state.
/// </summary>
public class ProcessRunTracker(
    IRepository<ProcessRun> processRunRepository,
    IRepository<ProcessRunLogItem> logItemRepository,
    TimeProvider timeProvider)
{
    public Task<ProcessRun> StartAsync(int processTypeId, CancellationToken ct) =>
        processRunRepository.AddAsync(new ProcessRun
        {
            ProcessTypeId = processTypeId,
            StatusId = CoreConstants.ProcessRunStatuses.Running,
            StartedAtUtc = timeProvider.GetUtcNow(),
        }, ct);

    public async Task CompleteAsync(long processRunId, bool succeeded, IEnumerable<ValidationError> errors, CancellationToken ct)
    {
        var logItems = errors.Select(e => ToLogItem(processRunId, e)).ToList();
        if (logItems.Count > 0)
        {
            await logItemRepository.AddRangeAsync(logItems, ct);
        }

        var run = await processRunRepository.GetByIdAsync(processRunId, ct)
            ?? throw new InvalidOperationException($"ProcessRun '{processRunId}' was not found.");

        run.StatusId = succeeded ? CoreConstants.ProcessRunStatuses.Succeeded : CoreConstants.ProcessRunStatuses.Failed;
        run.CompletedAtUtc = timeProvider.GetUtcNow();

        await processRunRepository.UpdateAsync(run, ct);
    }

    private ProcessRunLogItem ToLogItem(long processRunId, ValidationError error)
    {
        EntityReference.TryParse(error.Identifier, out var entityType, out var entityId);

        return new ProcessRunLogItem
        {
            ProcessRunId = processRunId,
            LevelId = MapSeverity(error.Severity),
            EntityType = entityType,
            EntityId = entityId,
            Field = string.IsNullOrEmpty(error.ErrorCode) ? null : error.ErrorCode,
            Message = error.ErrorMessage,
            CreatedAtUtc = timeProvider.GetUtcNow(),
        };
    }

    private static int MapSeverity(ValidationSeverity severity) => severity switch
    {
        ValidationSeverity.Warning => CoreConstants.ProcessRunLogLevels.Warning,
        ValidationSeverity.Info => CoreConstants.ProcessRunLogLevels.Info,
        _ => CoreConstants.ProcessRunLogLevels.Error,
    };
}
