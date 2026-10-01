namespace Web.Endpoints.Markets.Get;

public class GetMarketRequest
{
    public const string Route = "/markets/markets/{MarketId:int}";
    public static string BuildRoute(int marketId) => Route.Replace("{MarketId:int}", marketId.ToString());

    public int MarketId { get; set; }
}
