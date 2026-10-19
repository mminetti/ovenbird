using Core.MarketExchange;

namespace UseCases.Market.MarketDocuments.Export.Interfaces;

public record MarketDocumentExportResult(MarketDocument Document, IReadOnlyList<MarketDocumentItem> Items);
