namespace UseCases.Markets.Create;

public class CreateMarketHandler(IRepository<Core.Market.Market> repository)
{
    public async Task<Result<int>> Handle(CreateMarketCommand command, CancellationToken ct)
    {
        var market = new Core.Market.Market
        {
            Name = command.Name,
            Identifier = command.Identifier
        };

        var created = await repository.AddAsync(market, ct);

        return created.Id;
    }
}
