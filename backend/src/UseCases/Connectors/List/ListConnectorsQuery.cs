namespace UseCases.Connectors.List;

public record ListConnectorsQuery(
    int? Page = 1,
    int? PerPage = Constants.Pagination.DefaultPageSize,
    string? Search = null,
    string? OrderBy = null);
