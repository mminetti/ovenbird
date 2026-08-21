using UseCases.Common;

namespace UseCases.Security.Roles.List;

public record ListRolesQuery(int? Page = 1, int? PerPage = Constants.DEFAULT_PAGE_SIZE);
