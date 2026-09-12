using System.Linq.Expressions;
using Core.Security;
using Infrastructure.Data.Queries.Common;
using UseCases.Security.Roles;
using UseCases.Security.Roles.List;

namespace Infrastructure.Data.Queries.Security;

public class ListRolesQueryService(ReadDbContext db, IQueryPropertyMapper<Role> propertyMapper)
    : PagedQueryServiceBase<Role, RoleDto>(propertyMapper), IListRolesQueryService
{
    protected override IQueryable<Role> GetQuery() => db.Role;

    protected override Expression<Func<Role, RoleDto>> GetProjection() =>
        r => new RoleDto(r.Id, r.Name);

    protected override string GetDefaultOrderBy() => nameof(Role.Id);
}
