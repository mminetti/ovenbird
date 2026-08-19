using FluentValidation;

namespace Web.Security.Roles.Create;

public class CreateRoleRequest
{
    public const string Route = "/security/roles";

    public string Name { get; set; } = string.Empty;
}

public class CreateRoleValidator : Validator<CreateRoleRequest>
{
    public CreateRoleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);
    }
}
