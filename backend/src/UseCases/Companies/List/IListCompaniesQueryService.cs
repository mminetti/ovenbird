namespace UseCases.Companies.List;

public interface IListCompaniesQueryService
{
    Task<ItemPagedResult<CompanyDto>> ListAsync(int page, int perPage, string? search, string? orderBy, CancellationToken ct);
}
