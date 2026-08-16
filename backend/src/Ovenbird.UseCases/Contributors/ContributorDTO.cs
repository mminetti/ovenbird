using Ovenbird.Core.ContributorAggregate;

namespace Ovenbird.UseCases.Contributors;

public record ContributorDto(ContributorId Id, ContributorName Name, PhoneNumber PhoneNumber);
