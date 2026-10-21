using Core.Common.Constants;
using Core.ServiceDelivery.Events;
using Core.ServiceDelivery.Specifications;

namespace Core.ServiceDelivery.Handlers;

public class ServiceStartScheduledEventHandler(
    IRepository<ServiceStart> serviceStartRepository,
    IRepository<ServicePoint> servicePointRepository)
{
    public async Task Handle(ServiceStartScheduledEvent domainEvent, CancellationToken ct)
    {
        var serviceStart = await serviceStartRepository.GetByIdAsync(domainEvent.ServiceStartId, ct)
            ?? throw new InvalidOperationException($"ServiceStart '{domainEvent.ServiceStartId}' was not found.");

        serviceStart.ServiceDate = domainEvent.ServiceDate;
        serviceStart.ServiceOrderStatusId = Constants.ServiceOrderStatuses.Scheduled;

        await serviceStartRepository.UpdateAsync(serviceStart, ct);

        var servicePoint = await servicePointRepository.FirstOrDefaultAsync(
            new ServicePointByIdentifierAndCommoditySpec(domainEvent.ServicePointIdentifier, domainEvent.CommodityId), ct)
            ?? throw new InvalidOperationException($"ServicePoint '{domainEvent.ServicePointIdentifier}' was not found.");

        servicePoint.CustomerClassId = domainEvent.CustomerClassId;
        servicePoint.LoadProfileId = domainEvent.LoadProfileId;
        servicePoint.ReadCycleId = domainEvent.ReadCycleId;
        servicePoint.IsAmsMeter = domainEvent.IsAmsMeter;

        servicePoint.AddMeter(domainEvent.MeterIdentifier);

        var premise = servicePoint.Premise;
        premise.AddressLine1 = domainEvent.AddressLine1;
        premise.City = domainEvent.City;
        premise.State = domainEvent.State;
        premise.Zip = domainEvent.Zip;
        premise.Zip4 = domainEvent.Zip4;
        premise.CountyId = domainEvent.CountyId;

        await servicePointRepository.UpdateAsync(servicePoint, ct);
    }
}
