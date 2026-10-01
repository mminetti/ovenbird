namespace UseCases.Settings.Configurations.List;

public record ListConfigurationsQuery(
    int? Page = 1,
    int? PerPage = Constants.Pagination.DefaultPageSize,
    string? Search = null,
    string? OrderBy = null);
