using System.Xml.Linq;
using Core.MarketExchange;

namespace Infrastructure.Services.Market.BigData.Readers;

public interface IMarketDocumentTransactionReader
{
    MarketDocumentItem Read(XElement transactionElement);
}
