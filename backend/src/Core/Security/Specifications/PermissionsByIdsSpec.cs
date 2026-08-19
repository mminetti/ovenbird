namespace Core.Security.Specifications;

public class PermissionsByIdsSpec : Specification<Permission>
{
    public PermissionsByIdsSpec(IReadOnlyList<int> ids) =>
        Query.Where(permission => ids.Contains(permission.Id));
}
