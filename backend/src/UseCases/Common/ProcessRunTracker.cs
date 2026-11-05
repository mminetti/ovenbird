using Core.Common;
using Core.Processing;

namespace UseCases.Common;

public class ProcessRunTracker(
    IRepository<ProcessRun> processRunRepository,
    CaseRecorder caseRecorder,
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
        IEnumerable<ValidationError> errors,
        CancellationToken ct)
    {
        var run = await processRunRepository.GetByIdAsync(processRunId, ct)
            ?? throw new InvalidOperationException($"ProcessRun '{processRunId}' was not found.");

        run.StatusId = errors.Any() ? Constants.ProcessRunStatuses.Failed : Constants.ProcessRunStatuses.Succeeded;
        run.CompletedAtUtc = timeProvider.GetUtcNow();

        await processRunRepository.UpdateAsync(run, ct);

        await caseRecorder.RecordAsync(Constants.CaseEntityTypes.ProcessRun, processRunId, errors, ct);
    }
}
