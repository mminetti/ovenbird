namespace Core.Security.Specifications;

public class UserWithRolesByIdSpec : Specification<User>
{
    public UserWithRolesByIdSpec(int userId) =>
        Query
            .Where(user => user.Id == userId)
            .Include(user => user.Roles);
}
