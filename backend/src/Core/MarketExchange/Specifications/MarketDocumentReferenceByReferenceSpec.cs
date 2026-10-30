namespace Core.MarketExchange.Specifications;

public class MarketDocumentReferenceByReferenceSpec : Specification<MarketDocumentReference>
{
    public MarketDocumentReferenceByReferenceSpec(int referenceTypeId, long referenceId) =>
        Query
            .Where(reference => reference.ReferenceTypeId == referenceTypeId && reference.ReferenceId == referenceId)
            .Include(reference => reference.MarketDocumentItem)
                .ThenInclude(item => item.MarketDocument);
}
