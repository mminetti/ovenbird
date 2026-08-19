namespace Core.Security.Specifications;

public class RolesByIdsSpec : Specification<Role>
{
    public RolesByIdsSpec(IReadOnlyList<int> ids) =>
        Query.Where(role => ids.Contains(role.Id));
}
