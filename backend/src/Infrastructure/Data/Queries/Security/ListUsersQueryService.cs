using System.Linq.Expressions;
using Core.Security;
using Infrastructure.Data.Queries.Common;
using UseCases.Security.Users;
using UseCases.Security.Users.List;

namespace Infrastructure.Data.Queries.Security;

public class ListUsersQueryService(ReadDbContext db, IQueryPropertyMapper<User> propertyMapper)
    : PagedQueryServiceBase<User, UserDto>(propertyMapper), IListUsersQueryService
{
    protected override IQueryable<User> GetQuery() => db.User;

    protected override Expression<Func<User, UserDto>> GetProjection() =>
        u => new UserDto(u.Id, u.Name, u.Email, u.IsActive);

    protected override string GetDefaultOrderBy() => nameof(User.Id);
}
