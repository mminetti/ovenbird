using Core.Common;
using Core.Processing;

namespace UseCases.Common;

public class ProcessRunTracker(
    IRepository<ProcessRun> processRunRepository,
    IRepository<Case> caseRepository,
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
        var run = await processRunRepository.GetByIdAsync(processRunId, ct)
            ?? throw new InvalidOperationException($"ProcessRun '{processRunId}' was not found.");

        run.StatusId = succeeded ? Constants.ProcessRunStatuses.Succeeded : Constants.ProcessRunStatuses.Failed;
        run.CompletedAtUtc = timeProvider.GetUtcNow();

        await processRunRepository.UpdateAsync(run, ct);

        var @case = CaseFactory.Create(Constants.CaseEntityTypes.ProcessRun, processRunId, errors);
        if (@case is not null)
        {
            await caseRepository.AddAsync(@case, ct);
        }
    }
}
