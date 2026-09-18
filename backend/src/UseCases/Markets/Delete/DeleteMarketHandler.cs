namespace UseCases.Markets.Delete;

public class DeleteMarketHandler(IRepository<Core.Market.Market> repository)
{
    public async Task<Result> Handle(DeleteMarketCommand command, CancellationToken ct)
    {
        var market = await repository.GetByIdAsync(command.MarketId, ct);

        if (market is null)
        {
            return Result.NotFound();
        }

        await repository.DeleteAsync(market, ct);

        return Result.Success();
    }
}
