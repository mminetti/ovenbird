using FluentValidation;

namespace Web.Security.Modules.Update;

public class UpdateModuleRequest
{
    public const string Route = "/security/modules/{ModuleId:int}";
    public static string BuildRoute(int moduleId) => Route.Replace("{ModuleId:int}", moduleId.ToString());

    public int ModuleId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class UpdateModuleValidator : Validator<UpdateModuleRequest>
{
    public UpdateModuleValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.ModuleId)
            .Must((args, moduleId) => args.Id == moduleId)
            .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");
    }
}
