namespace UseCases.DataLists.Get;

public interface IGetDataListQueryService
{
    Task<IReadOnlyList<ValuePairDto>> GetListAsync(DataListType type, CancellationToken ct);
}
