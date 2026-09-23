using FluentValidation;

namespace Web.Security.Users.Create;

public class CreateUserRequest
{
    public const string Route = "/security/users";

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ExternalIdentifier { get; set; } = string.Empty;
    public IReadOnlyList<int> RoleIds { get; set; } = [];
}

public class CreateUserValidator : Validator<CreateUserRequest>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("A valid email address is required.")
            .MaximumLength(200);

        RuleFor(x => x.ExternalIdentifier)
            .NotEmpty().WithMessage("ExternalIdentifier is required.")
            .MaximumLength(200);

        RuleForEach(x => x.RoleIds)
            .GreaterThan(0).WithMessage("Each role ID must be greater than 0.");
    }
}
