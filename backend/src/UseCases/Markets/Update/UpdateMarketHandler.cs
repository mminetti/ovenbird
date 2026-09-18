namespace UseCases.Markets.Update;

public class UpdateMarketHandler(IRepository<Core.Market.Market> repository)
{
    public async Task<Result> Handle(UpdateMarketCommand command, CancellationToken ct)
    {
        var market = await repository.GetByIdAsync(command.MarketId, ct);

        if (market is null)
        {
            return Result.NotFound();
        }

        market.Name = command.Name;
        market.Identifier = command.Identifier;

        await repository.UpdateAsync(market, ct);

        return Result.Success();
    }
}
