using System.Xml.Linq;
using Core.Market;

namespace UseCases.Market.MarketDocuments.Import.BigData.Readers;

public interface IMarketDocumentTransactionReader
{
    MarketDocumentItem Read(XElement transactionElement);
}
