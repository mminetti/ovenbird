namespace UseCases.Markets.Create;

public record CreateMarketCommand(
    string Name,
    string Identifier);
