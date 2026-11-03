namespace Core.MarketExchange.Specifications;

public class MarketDocumentEventByReferenceSpec : Specification<MarketDocumentEvent>
{
    public MarketDocumentEventByReferenceSpec(int eventTypeId, long referenceId) =>
        Query
            .Where(e => e.EventTypeId == eventTypeId && e.ReferenceId == referenceId)
            .Include(e => e.MarketDocumentItem)
                .ThenInclude(item => item.MarketDocument);
}
