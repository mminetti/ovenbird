using Core.Common.Constants;
using Core.ServiceDelivery;
using Core.ServiceDelivery.Events;
using Core.ServiceDelivery.Handlers;

namespace UnitTests.Core.ServiceDelivery.Handlers;

public class ServiceStartScheduledEventHandlerHandle
{
    private readonly ServiceStartScheduledEventHandler _handler = new();

    [Fact]
    public async Task SchedulesServiceStart()
    {
        var serviceStart = new ServiceStart { Id = 99 };
        var serviceDate = new DateOnly(2025, 11, 12);

        await _handler.Handle(new ServiceStartScheduledEvent(serviceStart, serviceDate, marketDocumentItemId: 42), CancellationToken.None);

        serviceStart.ServiceDate.ShouldBe(serviceDate);
        serviceStart.ServiceOrderStatusId.ShouldBe(Constants.ServiceOrderStatuses.Scheduled);
    }
}
