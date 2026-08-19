using FluentValidation;

namespace Web.Security.Permissions.Update;

public class UpdatePermissionRequest
{
    public const string Route = "/security/permissions/{PermissionId:int}";
    public static string BuildRoute(int permissionId) => Route.Replace("{PermissionId:int}", permissionId.ToString());

    public int PermissionId { get; set; }
    public int Id { get; set; }
    public int ModuleId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class UpdatePermissionValidator : Validator<UpdatePermissionRequest>
{
    public UpdatePermissionValidator()
    {
        RuleFor(x => x.ModuleId)
            .GreaterThan(0).WithMessage("ModuleId is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.PermissionId)
            .Must((args, permissionId) => args.Id == permissionId)
            .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");
    }
}
