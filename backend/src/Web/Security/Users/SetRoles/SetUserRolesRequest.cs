using FluentValidation;

namespace Web.Security.Users.SetRoles;

public class SetUserRolesRequest
{
    public const string Route = "/security/users/{UserId:int}/roles";

    public int UserId { get; set; }
    public IReadOnlyList<int> RoleIds { get; set; } = [];
}

public class SetUserRolesValidator : Validator<SetUserRolesRequest>
{
    public SetUserRolesValidator()
    {
        RuleForEach(x => x.RoleIds)
            .GreaterThan(0).WithMessage("Each role ID must be greater than 0.");
    }
}
