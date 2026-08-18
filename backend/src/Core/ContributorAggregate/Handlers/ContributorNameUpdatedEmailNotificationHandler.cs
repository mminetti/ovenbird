using Core.ContributorAggregate.Events;
using Core.Interfaces;

namespace Core.ContributorAggregate.Handlers;

public class ContributorNameUpdatedEmailNotificationHandler(
  ILogger<ContributorNameUpdatedEmailNotificationHandler> logger,
  IEmailSender emailSender)
{
    public async Task Handle(ContributorNameUpdatedEvent domainEvent, CancellationToken ct)
    {
        logger.LogInformation("Handling Contributor Name Updated event for {contributorId}", domainEvent.Contributor.Id);

        await emailSender.SendEmailAsync("to@test.com",
                                         "from@test.com",
                                         $"Contributor {domainEvent.Contributor.Id} Name Updated",
    $"Contributor with id {domainEvent.Contributor.Id} had their name updated to {domainEvent.Contributor.Name}.");
    }
}
