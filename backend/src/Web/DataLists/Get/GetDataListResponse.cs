namespace Web.DataLists.Get;

public record ValuePairRecord(string Id, string Name);

public record GetDataListResponse(string Type, IReadOnlyList<ValuePairRecord> Items);
