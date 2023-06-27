using System.Diagnostics;
using TestExchange.Domain;

namespace TestExchange.Application
{
    public class CryptoExchangeStore
    {
        private OrderBookReader reader;
        public Dictionary<string, СryptoExchange> Exchanges = new Dictionary<string, СryptoExchange>();
        public List<Order> FlattenedAsks = new List<Order>();
        public List<Order> FlattenedBids = new List<Order>();

        public CryptoExchangeStore(OrderBookReader reader)
        {
            this.reader = reader;
        }

        public СryptoExchange Create(string Id, OrderBook orderBook)
        {
            decimal moneyBalance = 5;   //TODO: Need source from wallet
            decimal BTCBalance = 5;     //TODO: Need source from wallet

            return new СryptoExchange(Id, orderBook, moneyBalance, BTCBalance);
        }

        public void FulFillExchanges()
        {
            var orderbooks = reader.Read();

            var t1 = Stopwatch.StartNew();
            Exchanges.Clear();
            foreach (var orderbook in orderbooks)
            {
                var exchange = Create(orderbook.Key, orderbook.Value);
                Exchanges.Add(orderbook.Key, exchange);
                FlattenedAsks.AddRange(exchange.OrderBook.Asks);
                FlattenedBids.AddRange(exchange.OrderBook.Bids);
            }
            t1.Stop();
            Console.WriteLine($"Read file duration: {t1.ElapsedMilliseconds}");
            t1.Restart();

            FlattenedBids.Sort((a, b) => b.Price.CompareTo(a.Price));
            FlattenedAsks.Sort((a, b) => a.Price.CompareTo(b.Price));
            
            t1.Stop();
            Console.WriteLine($"Sorting duration: {t1.ElapsedMilliseconds}");
        }
    }
}
