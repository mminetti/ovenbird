namespace Core.ServiceDelivery.Specifications;

public class ServicePointByIdentifierAndCommoditySpec : Specification<ServicePoint>
{
    public ServicePointByIdentifierAndCommoditySpec(string identifier, int commodityId) =>
        Query
            .Include(x => x.Meters)
            .Include(x => x.Premise)
            .Where(x =>
                x.Identifier == identifier &&
                x.CommodityId == commodityId);
}
