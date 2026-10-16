namespace UseCases.Market.MarketDocuments.Import.Interfaces;

public class EdiImportHandlerResolver(IEnumerable<IEdiImportHandler> handlers)
{
    public IEdiImportHandler Resolve(string identifier)
    {
        var match = handlers.FirstOrDefault(h =>
            string.Equals(h.Identifier, identifier, StringComparison.OrdinalIgnoreCase));

        return match ??
            throw new InvalidOperationException($"Edi Import Handler couldn't resolve identifier '{identifier}'.");
    }
}
