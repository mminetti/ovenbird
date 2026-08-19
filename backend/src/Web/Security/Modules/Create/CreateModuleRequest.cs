using FluentValidation;

namespace Web.Security.Modules.Create;

public class CreateModuleRequest
{
    public const string Route = "/security/modules";

    public string Name { get; set; } = string.Empty;
}

public class CreateModuleValidator : Validator<CreateModuleRequest>
{
    public CreateModuleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);
    }
}
