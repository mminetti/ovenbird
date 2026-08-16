using Ovenbird.Core.ContributorAggregate;

namespace Ovenbird.UseCases.Contributors.Get;

public record GetContributorQuery(ContributorId ContributorId) : IQuery<Result<ContributorDto>>;
