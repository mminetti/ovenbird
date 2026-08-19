namespace Core.Security.Specifications;

public class RoleWithPermissionsByIdSpec : Specification<Role>
{
    public RoleWithPermissionsByIdSpec(int roleId) =>
        Query
            .Where(role => role.Id == roleId)
            .Include(role => role.Permissions);
}
