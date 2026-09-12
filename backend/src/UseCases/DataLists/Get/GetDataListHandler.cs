namespace UseCases.DataLists.Get;

public class GetDataListHandler(IGetDataListQueryService query)
{
    public async Task<Result<DataListResponse>> Handle(GetDataListQuery request, CancellationToken ct)
    {
        var items = await query.GetListAsync(request.Type, ct);

        return Result.Success(new DataListResponse(request.Type.ToString(), items));
    }
}
