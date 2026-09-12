namespace UseCases.DataLists;

/// <summary>
/// Reference data for a specific data list type.
/// </summary>
public record DataListResponse(string Type, IReadOnlyList<ValuePairDto> Items);
