namespace Core.Security.Specifications;

public class UserWithRolesAndPermissionsByExternalIdentifierSpec : Specification<User>
{
    public UserWithRolesAndPermissionsByExternalIdentifierSpec(string externalIdentifier) =>
        Query
            .Where(user => user.ExternalIdentifier == externalIdentifier)
            .Include(user => user.Roles)
            .ThenInclude(role => role.Permissions)
            .ThenInclude(permission => permission.Module);
}
