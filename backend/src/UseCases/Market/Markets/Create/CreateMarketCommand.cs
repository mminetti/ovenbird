namespace UseCases.Market.Markets.Create;

public record CreateMarketCommand(
    string Name,
    string Identifier);
