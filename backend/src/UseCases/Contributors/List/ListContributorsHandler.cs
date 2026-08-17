namespace UseCases.Contributors.List;

public class ListContributorsHandler
{
    private readonly IListContributorsQueryService _query;

    public ListContributorsHandler(IListContributorsQueryService query)
    {
        _query = query;
    }

    public async Task<Result<PagedResult<ContributorDto>>> Handle(ListContributorsQuery request,
                                                                       CancellationToken cancellationToken)
    {

        var result = await _query.ListAsync(request.Page ?? 1, request.PerPage ?? Constants.DEFAULT_PAGE_SIZE, cancellationToken);

        return Result.Success(result);
    }
}
