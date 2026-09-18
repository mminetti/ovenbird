using Core.Shared;
using Core.Shared.Specifications;

namespace UseCases.Companies.Get;

public class GetCompanyHandler(IReadRepository<Company> repository)
{
    public async Task<Result<CompanyDto>> Handle(GetCompanyQuery request, CancellationToken ct)
    {
        var entity = await repository.FirstOrDefaultAsync(new CompanyByIdSpec(request.CompanyId), ct);

        if (entity is null)
        {
            return Result.NotFound();
        }

        return Result.Success(new CompanyDto(
            entity.Id,
            entity.Name,
            entity.MarketId,
            entity.Market.Name,
            entity.TimeZoneId));
    }
}
