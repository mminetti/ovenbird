using Core.Shared;
using Core.Shared.Specifications;

namespace UseCases.Companies.Update;

public class UpdateCompanyHandler(IRepository<Company> repository)
{
    public async Task<Result> Handle(UpdateCompanyCommand command, CancellationToken ct)
    {
        var company = await repository.FirstOrDefaultAsync(new CompanyByIdSpec(command.CompanyId), ct);

        if (company is null)
        {
            return Result.NotFound();
        }

        company.Name = command.Name;
        company.MarketId = command.MarketId;
        company.TimeZoneId = command.TimeZoneId;

        await repository.UpdateAsync(company, ct);

        return Result.Success();
    }
}
