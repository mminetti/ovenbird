namespace UseCases.Common;

public record ItemPagedResult<T>(
  IReadOnlyList<T> Items,
  int Page,
  int PerPage,
  int TotalCount,
  int TotalPages);
