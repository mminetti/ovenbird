using System.Xml.Linq;
using Core.Market;

namespace Infrastructure.Services.Market.BigData.Readers;

public interface IMarketDocumentTransactionReader
{
    MarketDocumentItem Read(XElement transactionElement);
}
