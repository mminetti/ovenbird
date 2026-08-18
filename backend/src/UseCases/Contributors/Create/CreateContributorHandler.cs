using Core.ContributorAggregate;

namespace UseCases.Contributors.Create;

public class CreateContributorHandler(IRepository<Contributor> _repository)
{
    public async Task<Result<int>> Handle(CreateContributorCommand command, CancellationToken ct)
    {
        var newContributor = new Contributor(command.Name);

        if (!string.IsNullOrEmpty(command.PhoneNumber))
        {
            var phoneNumber = new PhoneNumber("+1", command.PhoneNumber, string.Empty);
            newContributor.UpdatePhoneNumber(phoneNumber);
        }

        var createdItem = await _repository.AddAsync(newContributor, ct);

        return createdItem.Id;
    }
}
