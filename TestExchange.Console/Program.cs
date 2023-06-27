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
            store.FulFillExchanges();

            Console.WriteLine($"End {store.Exchanges.Count} and {store.FlattenedAsks.Count}");
        }

        private static void PrintAsksBids(OrderBook orderbook)
        {
            Console.WriteLine("Asks:");

            foreach (var item in orderbook.Asks)
            {
                Console.WriteLine($"{item.Price} for {item.Amount}");
            }

            Console.WriteLine("Bids:");
            foreach (var item in orderbook.Bids)
            {
                Console.WriteLine($"{item.Price} for {item.Amount}");
            }
        }

        private static void PrintPurchaseList(PurchaseList list, decimal balance)
        {
            for (int i = 0; i < list.Items.Count; i++)
            {
                balance -= list.Items[i].TotalCost;
                Console.WriteLine($"Ask with price:{list.Items[i].Price} was bought with amount {list.Items[i].Amount}");
                Console.WriteLine($"Total cost: {list.Items[i].TotalCost}. Your balance: {balance}");
            }
        }
    }
}