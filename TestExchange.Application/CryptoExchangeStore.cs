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

        public void FulFillExchanges(Wallet wallet)
        {
            var orderbooks = reader.Read();

            var t1 = Stopwatch.StartNew();
            Exchanges.Clear();
            foreach (var orderbook in orderbooks)
            {
                var exchange = Create(orderbook.Key, orderbook.Value, wallet);
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

        private CryptoExchange Create(string Id, OrderBook orderbook, Wallet wallet)
        {
            decimal money = 0m;
            if (!wallet.EmptyWallet(Id))
                money = wallet.Money(Id);

            decimal amount = 0m;
            if (!wallet.EmptyCoins(Id))
                amount = wallet.Coins(Id);

            return new CryptoExchange(Id, orderbook, money, amount);
        }
    }
}
