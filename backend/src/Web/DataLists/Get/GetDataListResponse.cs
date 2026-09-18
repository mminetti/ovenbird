namespace Web.DataLists.Get;

public record ValuePairRecord(string Id, string Name, string? ParentId = null);

public record GetDataListResponse(string Type, IReadOnlyList<ValuePairRecord> Items);
