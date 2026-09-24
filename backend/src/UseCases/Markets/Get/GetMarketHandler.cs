namespace UseCases.Markets.Get;

public class GetMarketHandler(IReadRepository<Core.Market.Market> repository)
{
    public async Task<Result<MarketDto>> Handle(GetMarketQuery request, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(request.MarketId, ct);

        if (entity is null)
        {
            return Result.NotFound();
        }

        return Result.Success(new MarketDto(entity.Id, entity.Name, entity.Identifier, entity.LastModifiedAtUtc, entity.LastModifiedBy));
    }
}
