namespace UseCases.Settings.Connectors.List;

public interface IListConnectorsQueryService
{
    Task<ItemPagedResult<ConnectorDto>> ListAsync(int page, int perPage, string? search, string? orderBy, CancellationToken ct);
}
