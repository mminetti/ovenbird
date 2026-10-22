using System.Xml.Linq;
using Core.MarketExchange;

namespace Infrastructure.Services.ServiceDelivery.BigData.Readers;

public interface IMarketDocumentTransactionReader
{
    MarketDocumentItem Read(XElement transactionElement);
}
