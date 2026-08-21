using UseCases.Common;

namespace UseCases.Security.Modules.List;

public record ListModulesQuery(int? Page = 1, int? PerPage = Constants.DEFAULT_PAGE_SIZE);
