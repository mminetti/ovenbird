using FluentValidation;

namespace Web.Security.Permissions.Create;

public class CreatePermissionRequest
{
    public const string Route = "/security/permissions";

    public string Name { get; set; } = string.Empty;
    public int ModuleId { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CreatePermissionValidator : Validator<CreatePermissionRequest>
{
    public CreatePermissionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.ModuleId)
            .GreaterThan(0).WithMessage("Module is required.");

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
