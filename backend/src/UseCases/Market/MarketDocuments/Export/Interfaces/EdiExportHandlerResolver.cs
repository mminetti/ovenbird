namespace UseCases.Market.MarketDocuments.Export.Interfaces;

public class EdiExportHandlerResolver(IEnumerable<IEdiExportHandler> handlers)
{
    public IEdiExportHandler Resolve(string identifier)
    {
        var match = handlers.FirstOrDefault(h =>
            string.Equals(h.Identifier, identifier, StringComparison.OrdinalIgnoreCase));

        return match ??
            throw new InvalidOperationException($"Edi Export Handler couldn't resolve identifier '{identifier}'.");
    }
}
