using Ovenbird.Core.ContributorAggregate;

namespace Ovenbird.UseCases.Contributors.Update;

public record UpdateContributorCommand(ContributorId ContributorId, ContributorName NewName) : ICommand<Result<ContributorDto>>;
