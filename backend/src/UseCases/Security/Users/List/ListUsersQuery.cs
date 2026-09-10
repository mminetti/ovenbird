using UseCases.Common;

namespace UseCases.Security.Users.List;

public record ListUsersQuery(int? Page = 1, int? PerPage = Constants.Pagination.DefaultPageSize);
