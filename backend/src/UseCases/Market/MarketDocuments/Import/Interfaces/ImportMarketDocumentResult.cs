namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public record ImportMarketDocumentResult(
    IReadOnlyList<string> Imported,
    IReadOnlyList<ValidationError> Errors);
