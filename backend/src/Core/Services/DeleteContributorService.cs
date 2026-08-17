using Core.ContributorAggregate;
using Core.ContributorAggregate.Events;
using Core.Interfaces;

namespace Core.Services;

/// <summary>
/// This is here mainly so there's an example of a domain service
/// and also to demonstrate how to fire domain events from a service.
/// </summary>
/// <param name="_repository"></param>
/// <param name="_mediator"></param>
/// <param name="_logger"></param>
public class DeleteContributorService(IRepository<Contributor> _repository,
  IMessageBus _bus,
  ILogger<DeleteContributorService> _logger) : IDeleteContributorService
{
    public async ValueTask<Result> DeleteContributor(int contributorId)
    {
        _logger.LogInformation("Deleting Contributor {contributorId}", contributorId);
        Contributor? aggregateToDelete = await _repository.GetByIdAsync(contributorId);
        if (aggregateToDelete == null) return Result.NotFound();

        await _repository.DeleteAsync(aggregateToDelete);
        var domainEvent = new ContributorDeletedEvent(contributorId);
        await _bus.PublishAsync(domainEvent);

        return Result.Success();
    }
}
