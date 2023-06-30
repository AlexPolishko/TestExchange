using TestExchange.Domain;

namespace TestExchange.Application
{
    public class CryptoExchangeStore : ICryptoExchangeStore
    {
        private readonly IOrderBookReader reader;
        public List<string> ExchangesId { get; } = new List<string>();
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

            ExchangesId.Clear();
            foreach (var orderbook in orderbooks)
            {
                ExchangesId.Add(orderbook.Key);
                FlattenedAsks.AddRange(orderbook.Value.Asks);
                FlattenedBids.AddRange(orderbook.Value.Bids);
            }

            FlattenedBids.Sort((a, b) => b.Price.CompareTo(a.Price));
            FlattenedAsks.Sort((a, b) => a.Price.CompareTo(b.Price));
        }
    }
}
