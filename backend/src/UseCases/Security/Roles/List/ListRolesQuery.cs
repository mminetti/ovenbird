using UseCases.Common;

namespace UseCases.Security.Roles.List;

public record ListRolesQuery(int? Page = 1, int? PerPage = Constants.Pagination.DefaultPageSize);
