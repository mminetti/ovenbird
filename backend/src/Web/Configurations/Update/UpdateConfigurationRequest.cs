using FluentValidation;

namespace Web.Configurations.Update;

public class UpdateConfigurationRequest
{
    public const string Route = "/settings/configurations/{ConfigurationId:int}";
    public static string BuildRoute(int configurationId) => Route.Replace("{ConfigurationId:int}", configurationId.ToString());

    public int ConfigurationId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ConfigurationTypeId { get; set; }
    public int? CompanyId { get; set; }
    public IReadOnlyList<int> ConnectorIds { get; set; } = [];
    public IReadOnlyList<UpdateConfigurationFieldRequest> Fields { get; set; } = [];
}

public class UpdateConfigurationFieldRequest
{
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }
    public string Operation { get; set; } = string.Empty;
}

public class UpdateConfigurationFieldRequestValidator : Validator<UpdateConfigurationFieldRequest>
{
    public UpdateConfigurationFieldRequestValidator()
    {
        RuleFor(x => x.Operation)
            .Must(op => op is "create" or "update" or "delete")
            .WithMessage("Field operation must be 'create', 'update', or 'delete'.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Field name is required.")
            .MaximumLength(200)
            .When(x => x.Operation is "create" or "update");

        RuleFor(x => x.Id)
            .NotNull().WithMessage("Field Id is required for update or delete operations.")
            .When(x => x.Operation is "update" or "delete");
    }
}

public class UpdateConfigurationValidator : Validator<UpdateConfigurationRequest>
{
    public UpdateConfigurationValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.ConfigurationTypeId)
            .GreaterThan(0).WithMessage("Configuration type is required.");

        RuleForEach(x => x.Fields).SetValidator(new UpdateConfigurationFieldRequestValidator());

        RuleFor(x => x.ConfigurationId)
            .Must((args, configurationId) => args.Id == configurationId)
            .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");
    }
}
