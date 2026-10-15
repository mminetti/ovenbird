using Core.Common.Constants;

namespace Core.MarketExchange.Specifications;

public class MarketDocumentItemsPendingExportSpec : Specification<MarketDocumentItem>
{
    public MarketDocumentItemsPendingExportSpec(string set) =>
        Query.Where(x =>
            x.Set == set &&
            x.MarketDocumentId == null &&
            x.MarketDocumentItemStatusId == Constants.MarketDocumentItemStatuses.New);
}
