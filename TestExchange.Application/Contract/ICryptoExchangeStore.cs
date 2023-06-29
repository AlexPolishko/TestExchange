using TestExchange.Domain;

namespace TestExchange.Application
{
    public interface ICryptoExchangeStore
    {
        Dictionary<string, OrderBook> Exchanges { get; }
        List<Order> FlattenedAsks { get; }
        List<Order> FlattenedBids { get; }
    }
}