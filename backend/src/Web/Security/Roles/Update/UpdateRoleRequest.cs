using FluentValidation;

namespace Web.Security.Roles.Update;

public class UpdateRoleRequest
{
    public const string Route = "/security/roles/{RoleId:int}";
    public static string BuildRoute(int roleId) => Route.Replace("{RoleId:int}", roleId.ToString());

    public int RoleId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
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
    }
}
