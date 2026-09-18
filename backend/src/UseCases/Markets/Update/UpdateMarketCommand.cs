namespace UseCases.Markets.Update;

public record UpdateMarketCommand(
    int MarketId,
    string Name,
    string Identifier);
