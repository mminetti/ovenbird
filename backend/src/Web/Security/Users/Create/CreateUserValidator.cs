using FluentValidation;

namespace Web.Security.Users.Create;

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
    }
}
