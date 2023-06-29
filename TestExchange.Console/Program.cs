using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestExchange.Application;
using TestExchange.Domain;

namespace TestExchange
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter 'buy' or 'sell' transaction direction:");

            var transactionDirection = ChooseDirection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var serviceProvider = new ServiceCollection()
                            .AddOptions()
                            .Configure<AppSettings>(configuration.GetSection("appsettings"))
                            .AddSingleton<IOrderBookReader, OrderBookReader>()
                            .AddSingleton<ICryptoExchangeStore, CryptoExchangeStore>()
                            .BuildServiceProvider();

            var store = serviceProvider.GetRequiredService<ICryptoExchangeStore>();
            PurchaseList result = new PurchaseList(0);
            var walletService = CreateWallet(store, transactionDirection == "sell");
            var amount = InputValue($"Enter how much BTC you want to {transactionDirection}:");
            var resolver = new Resolver(store, walletService);

            if (transactionDirection == "sell")
            {
                result = resolver.Sell(amount);
            }
            else
            {
                result = resolver.Buy(amount);
            }

            if (result.IsPurchaseSuccessful)
            {
                Console.WriteLine($"You cannot {transactionDirection} all ordered BTC");
            }
            else
            {
                foreach (var item in result.Items)
                {
                    Console.WriteLine($"You need to {transactionDirection} PRICE:{item.Price} AMOUNT:{item.Amount} on {item.ExchangeId} ");
                }
            }
        }

        private static WalletService CreateWallet(ICryptoExchangeStore store, bool isSell)
        {
            string cryptoExchangeID;
            var wallet = new WalletService();
            do
            {
                var term = isSell ? "BTC" : "Money";
                Console.WriteLine($"Enter cryptoExchangeID (decimal number) where you want to add your {term} or 'all' or 'random' or 'first' or 'last':");
                Console.WriteLine("Enter 'exit' for skip");
                do
                {
                    cryptoExchangeID = Console.ReadLine().ToLower();
                    if (decimal.TryParse(cryptoExchangeID, out decimal parsesomeresult))
                        break;

                } while (cryptoExchangeID != "all" && cryptoExchangeID != "random" && cryptoExchangeID != "first" && cryptoExchangeID != "last" && cryptoExchangeID != "exit");

                if (cryptoExchangeID == "exit") break;

                var amount = InputValue($"Enter amount of {term}:");

                if (cryptoExchangeID == "all")
                    wallet.SetMoneyCoinsToAllExchanges(store, isSell ? 0m : amount, isSell ? amount : 0m);
                if (cryptoExchangeID == "first")
                    wallet.SetMoneyCoinsToFirstExchange(store, isSell ? 0m : amount, isSell ? amount : 0m);
                if (cryptoExchangeID == "last")
                    wallet.SetMoneyCoinsToLasExchange(store, isSell ? 0m : amount, isSell ? amount : 0m);
                if (cryptoExchangeID == "random")
                    wallet.SetMoneyCoinsToRandomExchange(store, isSell ? 0m : amount, isSell ? amount : 0m);
                if (decimal.TryParse(cryptoExchangeID, out decimal parseresult))
                    wallet.SetMoneyCoinsToExchange(store, parseresult.ToString(), isSell ? 0m : amount, isSell ? amount : 0m);

            } while (cryptoExchangeID != "exit");
            return wallet;
        }

        private static string ChooseDirection()
        {
            string transactionDirection;
            do
            {
                transactionDirection = Console.ReadLine().ToLower();
            } while (transactionDirection != "buy" && transactionDirection != "sell");

            Console.WriteLine("You chooused: " + transactionDirection);
            return transactionDirection;
        }

        private static decimal InputValue(string message)
        {
            decimal value = 0m;
            bool isValid = false;

            while (!isValid)
            {
                Console.Write(message);
                string input = Console.ReadLine();

                isValid = decimal.TryParse(input, out value);

                if (!isValid)
                {
                    Console.WriteLine("Invalid input. Please enter a valid decimal value.");
                }
            }

            Console.WriteLine("Valid decimal value entered: " + value);
            return value;
        }
    }
}