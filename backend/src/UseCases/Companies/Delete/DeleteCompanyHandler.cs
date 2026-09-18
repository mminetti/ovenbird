using Core.Shared;

namespace UseCases.Companies.Delete;

public class DeleteCompanyHandler(IRepository<Company> repository)
{
    public async Task<Result> Handle(DeleteCompanyCommand command, CancellationToken ct)
    {
        var company = await repository.GetByIdAsync(command.CompanyId, ct);

        if (company is null)
        {
            return Result.NotFound();
        }

        await repository.DeleteAsync(company, ct);

        return Result.Success();
    }
}
