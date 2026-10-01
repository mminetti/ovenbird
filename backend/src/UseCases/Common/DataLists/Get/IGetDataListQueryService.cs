using UseCases.Common.DataLists;

namespace UseCases.Common.DataLists.Get;

public interface IGetDataListQueryService
{
    Task<IReadOnlyList<ValuePairDto>> GetListAsync(DataListType type, CancellationToken ct);
}
