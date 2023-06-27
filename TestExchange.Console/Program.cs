using System;
using TestExchange.Application;

namespace TestExchange
{ 
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start!");

            var reader = new OrderBookReader("order_books_data");
            var orderbooks = reader.Read();

            Random random = new Random();
            int randomIndex = random.Next(0, orderbooks.Count);
            string randomKey = orderbooks.Keys.ElementAt(randomIndex);

            var orderbook = orderbooks[randomKey];
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
    }
}