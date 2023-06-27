using TestExchange.Domain;

namespace TestExchange.Application
{
    public interface ICryptoExchangeStore
    {
        void FulFillExchanges();
        Dictionary<string, CryptoExchange> Exchanges { get; }
        List<Order> FlattenedAsks { get; }
        List<Order> FlattenedBids { get; }
    }
}