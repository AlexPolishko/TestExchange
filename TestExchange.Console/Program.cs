using TestExchange.Application;
using TestExchange.Domain;

namespace TestExchange
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start!");

            var reader = new OrderBookReader("order_books_data");
            var store = new CryptoExchangeStore(reader);
            var wallet = new Wallet();
            store.FulFillExchanges(wallet);

            Console.WriteLine($"End {store.Exchanges.Count} and {store.FlattenedAsks.Count}");
        }
    }
}