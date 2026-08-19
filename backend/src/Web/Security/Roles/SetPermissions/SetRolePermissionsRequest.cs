using FluentValidation;

namespace Web.Security.Roles.SetPermissions;

public class SetRolePermissionsRequest
{
    public const string Route = "/security/roles/{RoleId:int}/permissions";

    public int RoleId { get; set; }
    public IReadOnlyList<int> PermissionIds { get; set; } = [];
}

public class SetRolePermissionsValidator : Validator<SetRolePermissionsRequest>
{
    public SetRolePermissionsValidator()
    {
        RuleForEach(x => x.PermissionIds)
            .GreaterThan(0).WithMessage("Each permission ID must be greater than 0.");
    }
}
