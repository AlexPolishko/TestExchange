using System.Diagnostics;
using TestExchange.Domain;

namespace TestExchange.Application
{
    public class CryptoExchangeStore : ICryptoExchangeStore
    {
        private IOrderBookReader reader;
        public Dictionary<string, OrderBook> Exchanges { get; } = new Dictionary<string, OrderBook>();
        public List<Order> FlattenedAsks { get; } = new List<Order>();
        public List<Order> FlattenedBids { get; } = new List<Order>();

        public CryptoExchangeStore(IOrderBookReader reader)
        {
            this.reader = reader;
            FulFillExchanges();
        }

        private void FulFillExchanges()
        {
            var orderbooks = reader.Read();

            Exchanges.Clear();
            foreach (var orderbook in orderbooks)
            {
                Exchanges.Add(orderbook.Key, orderbook.Value);
                FlattenedAsks.AddRange(orderbook.Value.Asks);
                FlattenedBids.AddRange(orderbook.Value.Bids);
            }

            FlattenedBids.Sort((a, b) => b.Price.CompareTo(a.Price));
            FlattenedAsks.Sort((a, b) => a.Price.CompareTo(b.Price));
        }
    }
}
