using Core.ContributorAggregate;
using UseCases;
using UseCases.Contributors;
using UseCases.Contributors.List;

namespace Infrastructure.Data.Queries;

public class FakeListContributorsQueryService : IListContributorsQueryService
{
    public Task<PagedResult<ContributorDto>> ListAsync(int page, int perPage, CancellationToken ct)
    {
        var items = new List<ContributorDto>();
        for (int i = 1; i <= 25; i++)
        {
            var phone = new PhoneNumber("+1", "555", "1234567");
            items.Add(new ContributorDto(i, $"Fake {i}", phone));
        }

        int totalPages = (int)Math.Ceiling(items.Count / (double)perPage);
        var result = new PagedResult<ContributorDto>(items, page, perPage, items.Count, totalPages);
        return Task.FromResult(result);
    }
}
