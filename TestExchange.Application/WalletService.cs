using TestExchange.Domain;

namespace TestExchange.Application
{
    public class WalletService : IWalletService
    {
        private Wallet wallet;

        public WalletService()
        {
            wallet = new Wallet();
        }

        public void AddToAllExchange(ICryptoExchangeStore store, decimal money, decimal coins)
        {
            var moneyDictionary = new Dictionary<string, decimal>();
            var coinsDictionary = new Dictionary<string, decimal>();

            foreach (var item in store.Exchanges)
            {
                moneyDictionary.Add(item.Key, money);
                coinsDictionary.Add(item.Key, coins);
            }

            wallet = new Wallet(moneyDictionary, coinsDictionary);
        }

        public void ChangeFirstExchange(ICryptoExchangeStore store, decimal money, decimal coins)
        {
            var key = store.Exchanges.Keys.FirstOrDefault();
            wallet.AddMoney(key, money);
            wallet.AddCoins(key, coins);
        }

        public void ChangeLastExchange(ICryptoExchangeStore store, decimal money, decimal coins)
        {
            var key = store.Exchanges.Keys.LastOrDefault();
            wallet.AddMoney(key, money);
            wallet.AddCoins(key, coins);
        }

        public void ChangeRandomExchange(ICryptoExchangeStore store, decimal money, decimal coins)
        {
            Random random = new Random();
            int randomIndex = random.Next(store.Exchanges.Count);
            var key = store.Exchanges.Keys.ElementAt(randomIndex);
            wallet.AddMoney(key, money);
            wallet.AddCoins(key, coins);
        }

        public void ChangeExchange(ICryptoExchangeStore store, string exchangeId, decimal money, decimal coins)
        {

            if (!store.Exchanges.Keys.Contains(exchangeId))
            {
                return;
            }

            wallet.AddMoney(exchangeId, money);
            wallet.AddCoins(exchangeId, coins);
        }

        public Wallet GetWallet()
        {
            return wallet.Clone();
        }

        public void CreateWallet(Dictionary<string, decimal> money, Dictionary<string, decimal> coins)
        {
            wallet = new Wallet(money, coins);

        }
    }
}
