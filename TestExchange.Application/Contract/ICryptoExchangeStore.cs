using TestExchange.Domain;

namespace TestExchange.Application
{
    public interface ICryptoExchangeStore
    {
        Dictionary<string, CryptoExchange> Exchanges { get; }
        List<Order> FlattenedAsks { get; }
        List<Order> FlattenedBids { get; }
    }
}