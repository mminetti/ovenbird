using FluentValidation;

namespace Web.Security.Roles.Create;

public class CreateRoleRequest
{
    public const string Route = "/security/roles";

    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<int> PermissionIds { get; set; } = [];
}

public class CreateRoleValidator : Validator<CreateRoleRequest>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleForEach(x => x.PermissionIds)
            .GreaterThan(0).WithMessage("Each permission ID must be greater than 0.");
    }
}
