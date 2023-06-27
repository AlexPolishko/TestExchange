using TestExchange.Domain;

namespace TestExchange.Application
{
    public interface IOrderBookReader
    {
        Dictionary<string, OrderBook> Read();
    }
}