using FluentValidation;

namespace Web.Markets.Update;

public class UpdateMarketRequest
{
    public const string Route = "/markets/{MarketId:int}";
    public static string BuildRoute(int marketId) => Route.Replace("{MarketId:int}", marketId.ToString());

    public int MarketId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Identifier { get; set; } = string.Empty;
}

public class UpdateMarketValidator : Validator<UpdateMarketRequest>
{
    public UpdateMarketValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(250);

        RuleFor(x => x.Identifier)
            .NotEmpty().WithMessage("Identifier is required.")
            .MaximumLength(250);

        RuleFor(x => x.MarketId)
            .Must((args, marketId) => args.Id == marketId)
            .WithMessage("Route and body Ids must match; cannot update Id of an existing resource.");
    }
}
