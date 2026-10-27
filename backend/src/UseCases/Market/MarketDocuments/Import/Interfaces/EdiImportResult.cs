using Core.MarketExchange;

namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public record EdiImportResult(
    IReadOnlyList<MarketDocument> Documents,
    IReadOnlyList<ValidationError> Errors);
