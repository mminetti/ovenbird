using System.Linq.Expressions;
using Core.Security;
using Infrastructure.Data.Queries.Common;
using UseCases.Security.Permissions;
using UseCases.Security.Permissions.List;

namespace Infrastructure.Data.Queries.Security;

public class ListPermissionsQueryService(ReadDbContext db, IQueryPropertyMapper<Permission> propertyMapper)
    : PagedQueryServiceBase<Permission, PermissionDto>(propertyMapper), IListPermissionsQueryService
{
    protected override IQueryable<Permission> GetQuery() => db.Permission;

    protected override Expression<Func<Permission, PermissionDto>> GetProjection() =>
        p => new PermissionDto(p.Id, p.Name, p.Description);

    protected override string GetDefaultOrderBy() => nameof(Permission.Id);
}
