using System;
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
            var orderbooks = reader.Read();

            Random random = new Random();
            int randomIndex = random.Next(0, orderbooks.Count);
            string randomKey = orderbooks.Keys.ElementAt(randomIndex);

            var orderbook = orderbooks[randomKey];
            PrintAsksBids(orderbook);

            var resolver = new Resolver(orderbook);

            decimal input = 0;
            while (input != -1)
            {
                Console.WriteLine("Enter your balance (-1 to exit):");
                string inputString = Console.ReadLine();

                if (decimal.TryParse(inputString, out input))
                {
                    // Process the input digit
                    Console.WriteLine("You entered: " + input);
                    resolver.Buy(input);
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid balance.");
                }
            }
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
    }
}