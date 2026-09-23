using FluentValidation;

namespace Web.Security.Roles.Update;

public class UpdateRoleRequest
{
    public const string Route = "/security/roles/{RoleId:int}";
    public static string BuildRoute(int roleId) => Route.Replace("{RoleId:int}", roleId.ToString());

    public int RoleId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public IList<UpdateRolePermissionRequest> Permissions { get; set; } = [];
}

public record UpdateRolePermissionRequest
{
    public int PermissionId { get; set; }
    public string Operation { get; set; } = string.Empty;
}

public class UpdateRoleValidator : Validator<UpdateRoleRequest>
{
    public UpdateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.RoleId)
            .Must((args, roleId) => args.Id == roleId)
            .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");

        RuleForEach(x => x.Permissions).ChildRules(permission =>
        {
            permission.RuleFor(p => p.PermissionId)
                .GreaterThan(0).WithMessage("Each permission ID must be greater than 0.");

            permission.RuleFor(p => p.Operation)
                .Must(op => op.Equals("add", StringComparison.OrdinalIgnoreCase)
                            || op.Equals("remove", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Operation must be 'add' or 'remove'.");
        });
    }
}
