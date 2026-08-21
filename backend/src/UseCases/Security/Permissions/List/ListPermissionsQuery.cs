using UseCases.Common;

namespace UseCases.Security.Permissions.List;

public record ListPermissionsQuery(int? Page = 1, int? PerPage = Constants.DEFAULT_PAGE_SIZE);
