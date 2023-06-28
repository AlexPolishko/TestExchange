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
            var reader = new OrderBookReader("order_books_data");
            var store = new CryptoExchangeStore(reader);
            store.FulFillExchanges();
            PurchaseList result = new PurchaseList(0);
            var wallet = CreateWallet(store, transactionDirection == "sell");
            var amount = InputValue($"Enter how much BTC you want to {transactionDirection}:");
            var resolver = new Resolver(store, wallet);

            if (transactionDirection == "sell")
            {
                result = resolver.Sell(amount);
            }
            else
            {
                 result = resolver.Buy(amount);
            }

            if (result.RemainingAmount > 0)
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

        private static Wallet CreateWallet(CryptoExchangeStore store,  bool isSell)
        {
            string cryptoExchangeID;
            var wallet = new Wallet();
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

                var amount = InputValue("Enter amount of BTC:");

                if (cryptoExchangeID == "all")
                    wallet = FillAllWallet(store, isSell? 0m : amount, isSell ? amount : 0m);
                if (cryptoExchangeID == "first")
                    ChangeFirstExchange(store, wallet, isSell ? 0m : amount, isSell ? amount : 0m);
                if (cryptoExchangeID == "last")
                    ChangeLastExchange(store, wallet, isSell ? 0m : amount, isSell ? amount : 0m);
                if (cryptoExchangeID == "random")
                    ChangeRandomExchange(store, wallet, isSell ? 0m : amount, isSell ? amount : 0m);
                if (decimal.TryParse(cryptoExchangeID, out decimal parseresult))
                    ChangeExchange(store, wallet, parseresult.ToString(), isSell ? 0m : amount, isSell ? amount : 0m);

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

        private static Wallet FillAllWallet(CryptoExchangeStore store, decimal money, decimal coins)
        {
            var moneyDictionary = new Dictionary<string, decimal>();
            var coinsDictionary = new Dictionary<string, decimal>();

            foreach (var item in store.Exchanges)
            {
                moneyDictionary.Add(item.Key, money);
                coinsDictionary.Add(item.Key, coins);
            }
            Console.WriteLine($"All wallets with balance {money}$ / {coins}BTC");

            return new Wallet(moneyDictionary, coinsDictionary);

        }

        private static void ChangeFirstExchange(CryptoExchangeStore store, Wallet wallet, decimal money, decimal coins)
        {
            var key = store.Exchanges.Keys.FirstOrDefault();
            wallet.AddMoney(key, money);
            wallet.AddCoins(key, coins);

            Console.WriteLine($"First key: {key} with balance {money}$ / {coins}BTC");

        }

        private static void ChangeLastExchange(CryptoExchangeStore store, Wallet wallet, decimal money, decimal coins)
        {
            var key = store.Exchanges.Keys.LastOrDefault();
            wallet.AddMoney(key, money);
            wallet.AddCoins(key, coins);

            Console.WriteLine($"Last key: {key} with balance {money}$ / {coins}BTC");
        }
        private static void ChangeRandomExchange(CryptoExchangeStore store, Wallet wallet, decimal money, decimal coins)
        {
            Random random = new Random();
            int randomIndex = random.Next(store.Exchanges.Count);
            var key = store.Exchanges.Keys.ElementAt(randomIndex);
            wallet.AddMoney(key, money);
            wallet.AddCoins(key, coins);

            Console.WriteLine($"Random key: {key} with balance {money}$ / {coins}BTC");
        }
        private static void ChangeExchange(CryptoExchangeStore store, Wallet wallet, string exchangeId, decimal money, decimal coins)
        {

            if (!store.Exchanges.Keys.Contains(exchangeId))
            {
                Console.WriteLine($"Cann't found CryptoExchange with ID:{exchangeId}");
            }

            wallet.AddMoney(exchangeId, money);
            wallet.AddCoins(exchangeId, coins);

            Console.WriteLine($"Added {exchangeId} with balance {money}$ / {coins}BTC");
        }
    }
}