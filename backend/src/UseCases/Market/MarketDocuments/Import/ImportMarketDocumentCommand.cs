namespace UseCases.Market.MarketDocuments.Import;

public record ImportMarketDocumentCommand(string RemoteFilePath, int CompanyId);
