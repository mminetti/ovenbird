namespace UseCases.Market.Markets.List;

public record ListMarketsQuery(
    int? Page = 1,
    int? PerPage = Constants.Pagination.DefaultPageSize,
    string? Search = null,
    string? OrderBy = null);
