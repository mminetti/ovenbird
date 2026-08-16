namespace Core.ContributorAggregate.Events;

/// <summary>
/// A domain event that is dispatched whenever a contributor is deleted.
/// The DeleteContributorService is used to dispatch this event.
/// NOTE: Would prefer this be internal but Mediator Source Generator needs access to it from elsewhere.
/// </summary>
public sealed class ContributorDeletedEvent(int contributorId) : DomainEventBase
{
    public int ContributorId { get; init; } = contributorId;
}
