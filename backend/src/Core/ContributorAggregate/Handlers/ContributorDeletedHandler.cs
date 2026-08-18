using Core.ContributorAggregate.Events;
using Core.Interfaces;

namespace Core.ContributorAggregate.Handlers;

public class ContributorDeletedHandler(ILogger<ContributorDeletedHandler> logger,
  IEmailSender emailSender)
{
    public async Task Handle(ContributorDeletedEvent domainEvent, CancellationToken ct)
    {
        logger.LogInformation("Handling Contributed Deleted event for {contributorId}", domainEvent.ContributorId);

        await emailSender.SendEmailAsync("to@test.com",
                                         "from@test.com",
                                         "Contributor Deleted",
                                         $"Contributor with id {domainEvent.ContributorId} was deleted.");
    }
}
