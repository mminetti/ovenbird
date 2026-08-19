namespace Core.Security.Specifications;

public class PermissionByIdSpec : Specification<Permission>
{
    public PermissionByIdSpec(int permissionId) =>
        Query.Where(permission => permission.Id == permissionId);
}
