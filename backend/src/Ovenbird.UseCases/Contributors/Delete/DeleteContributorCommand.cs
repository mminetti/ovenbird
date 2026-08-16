using Ovenbird.Core.ContributorAggregate;

namespace Ovenbird.UseCases.Contributors.Delete;

public record DeleteContributorCommand(ContributorId ContributorId) : ICommand<Result>;
