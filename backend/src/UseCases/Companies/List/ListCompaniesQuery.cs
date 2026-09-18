namespace UseCases.Companies.List;

public record ListCompaniesQuery(
    int? Page = 1,
    int? PerPage = Constants.Pagination.DefaultPageSize,
    string? Search = null,
    string? OrderBy = null);
