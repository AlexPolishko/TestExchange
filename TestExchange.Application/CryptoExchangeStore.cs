using System.Diagnostics;
using TestExchange.Domain;

namespace TestExchange.Application
{
    public class CryptoExchangeStore : ICryptoExchangeStore
    {
        private IOrderBookReader reader;
        public Dictionary<string, CryptoExchange> Exchanges { get; } = new Dictionary<string, CryptoExchange>();
        public List<Order> FlattenedAsks { get; } = new List<Order>();
        public List<Order> FlattenedBids { get; } = new List<Order>();

        public CryptoExchangeStore(IOrderBookReader reader)
        {
            this.reader = reader;
        }

        public void FulFillExchanges()
        {
            var orderbooks = reader.Read();

            Exchanges.Clear();
            foreach (var orderbook in orderbooks)
            {
                var exchange = Create(orderbook.Key, orderbook.Value);
                Exchanges.Add(orderbook.Key, exchange);
                FlattenedAsks.AddRange(exchange.OrderBook.Asks);
                FlattenedBids.AddRange(exchange.OrderBook.Bids);
            }

            FlattenedBids.Sort((a, b) => b.Price.CompareTo(a.Price));
            FlattenedAsks.Sort((a, b) => a.Price.CompareTo(b.Price));
        }

        private CryptoExchange Create(string Id, OrderBook orderbook)
        {
            return new CryptoExchange(Id, orderbook);
        }
    }
}
