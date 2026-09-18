namespace UseCases.Configurations.List;

public interface IListConfigurationsQueryService
{
    Task<ItemPagedResult<ConfigurationDto>> ListAsync(int page, int perPage, string? search, string? orderBy, CancellationToken ct);
}
