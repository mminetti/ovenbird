namespace Core.Security.Specifications;

public class RoleByIdSpec : Specification<Role>
{
    public RoleByIdSpec(int roleId) =>
        Query.Where(role => role.Id == roleId);
}
