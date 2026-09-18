using Core.Shared;

namespace UseCases.Companies.Create;

public class CreateCompanyHandler(IRepository<Company> repository)
{
    public async Task<Result<int>> Handle(CreateCompanyCommand command, CancellationToken ct)
    {
        var company = new Company
        {
            Name = command.Name,
            MarketId = command.MarketId,
            TimeZoneId = command.TimeZoneId
        };

        var created = await repository.AddAsync(company, ct);

        return created.Id;
    }
}
