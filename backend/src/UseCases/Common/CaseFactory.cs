using Core.Common;

namespace UseCases.Common;

public static class CaseFactory
{
    public static Case? Create(int entityTypeId, long entityId, IEnumerable<ValidationError> errors)
    {
        var items = errors
            .Where(e => e.Severity == ValidationSeverity.Error)
            .Select(e => new CaseItem
            {
                SeverityId = Constants.ErrorSeverities.Error,
                Field = e.Identifier,
                Message = e.ErrorMessage,
            })
            .ToList();

        return items.Count == 0 ? null : Create(entityTypeId, entityId, items);
    }

    public static Case Create(int entityTypeId, long entityId, Exception exception) =>
        Create(entityTypeId, entityId,
        [
            new CaseItem
            {
                SeverityId = Constants.ErrorSeverities.Error,
                Field = string.Empty,
                Message = exception.Message,
            },
        ]);

    private static Case Create(int entityTypeId, long entityId, List<CaseItem> items) => new()
    {
        EntityTypeId = entityTypeId,
        EntityId = entityId,
        StatusId = Constants.CaseStatuses.Open,
        Items = items,
    };
}
