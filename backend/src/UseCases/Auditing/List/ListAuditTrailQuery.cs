namespace UseCases.Auditing.List;

public record ListAuditTrailQuery(
    string EntityType,
    string EntityId,
    int? Page = 1,
    int? PerPage = Constants.Pagination.DefaultPageSize);
