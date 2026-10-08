using System.Xml.Linq;
using Core.Market;

namespace UseCases.Market.MarketDocuments.Import.BigData.Readers;

public interface IMarketDocumentTransactionReader
{
    string Set { get; }
    MarketDocumentItem Read(XElement transactionElement);
}
