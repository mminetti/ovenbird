using Core.Subscription;

namespace Core.ServiceDelivery.Specifications;

public class ServiceByServicePointAndPeriodSpec : Specification<Service>
{
    public ServiceByServicePointAndPeriodSpec(string servicePointIdentifier, DateOnly periodStartDate, DateOnly periodEndDate) =>
        Query
            .Include(x => x.ServicePoint)
                .ThenInclude(x => x.Meters)
            .Where(x =>
                x.ServicePoint.Identifier == servicePointIdentifier &&
                x.StartDate <= periodStartDate &&
                (x.EndDate == null || x.EndDate >= periodEndDate));
}
