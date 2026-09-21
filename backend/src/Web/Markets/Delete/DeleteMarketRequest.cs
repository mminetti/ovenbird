namespace Web.Markets.Delete;

public class DeleteMarketRequest
{
    public const string Route = "/settings/markets/{MarketId:int}";
    public static string BuildRoute(int marketId) => Route.Replace("{MarketId:int}", marketId.ToString());

    public int MarketId { get; set; }
}
