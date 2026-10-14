namespace UseCases.Market.Markets.Create;

public class CreateMarketHandler(IRepository<Core.Service.Market> repository)
{
    public async Task<Result<int>> Handle(CreateMarketCommand command, CancellationToken ct)
    {
        var market = new Core.Service.Market
        {
            Name = command.Name,
            Identifier = command.Identifier
        };

        var created = await repository.AddAsync(market, ct);

        return created.Id;
    }
}
