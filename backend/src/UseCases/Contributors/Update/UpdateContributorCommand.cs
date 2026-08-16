using Core.ContributorAggregate;

namespace UseCases.Contributors.Update;

public record UpdateContributorCommand(int ContributorId, string NewName) : ICommand<Result<ContributorDto>>;
