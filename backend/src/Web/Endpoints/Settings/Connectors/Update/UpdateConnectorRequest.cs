using FluentValidation;

namespace Web.Endpoints.Settings.Connectors.Update;

public class UpdateConnectorRequest
{
    public const string Route = "/settings/connectors/{ConnectorId:int}";
    public static string BuildRoute(int connectorId) => Route.Replace("{ConnectorId:int}", connectorId.ToString());

    public int ConnectorId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int ConnectorImplementationId { get; set; }
    public IReadOnlyList<UpdateConnectorFieldRequest> Fields { get; set; } = [];
}

public class UpdateConnectorFieldRequest
{
    public int? Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Value { get; set; }
    public bool IsSecret { get; set; }
    public string Operation { get; set; } = string.Empty;
}

public class UpdateConnectorFieldRequestValidator : Validator<UpdateConnectorFieldRequest>
{
    public UpdateConnectorFieldRequestValidator()
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

public class UpdateConnectorValidator : Validator<UpdateConnectorRequest>
{
    public UpdateConnectorValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.ConnectorImplementationId)
            .GreaterThan(0).WithMessage("Connector implementation is required.");

        RuleForEach(x => x.Fields).SetValidator(new UpdateConnectorFieldRequestValidator());

        RuleFor(x => x.ConnectorId)
            .Must((args, connectorId) => args.Id == connectorId)
            .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");
    }
}
