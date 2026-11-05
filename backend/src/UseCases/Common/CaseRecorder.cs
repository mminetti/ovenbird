using Core.Common;

namespace UseCases.Common;

public class CaseRecorder(IRepository<Case> caseRepository)
{
    public Task RecordAsync(int entityTypeId, long entityId, IEnumerable<ValidationError> errors, CancellationToken ct) =>
        RecordAsync(CaseFactory.Create(entityTypeId, entityId, errors), ct);

    public Task RecordAsync(int entityTypeId, long entityId, Exception exception, CancellationToken ct) =>
        RecordAsync(CaseFactory.Create(entityTypeId, entityId, exception), ct);

    private Task RecordAsync(Case? @case, CancellationToken ct) =>
        @case is not null ? caseRepository.AddAsync(@case, ct) : Task.CompletedTask;
}
