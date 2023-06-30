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

        public void SetMoneyToAllExchanges(ICryptoExchangeStore store, decimal money)
        {
            var moneyDictionary = new Dictionary<string, decimal>();

            foreach (var item in store.ExchangesId)
            {
                moneyDictionary.Add(item, money);
            }

            wallet.Add(moneyDictionary, null);
        }

        public void SetCoinsToAllExchanges(ICryptoExchangeStore store, decimal coins)
        {
            var coinsDictionary = new Dictionary<string, decimal>();

            foreach (var item in store.ExchangesId)
            {
                coinsDictionary.Add(item, coins);
            }

            wallet.Add(null, coinsDictionary);
        }

        public void SetMoneyCoinsToFirstExchange(ICryptoExchangeStore store, decimal money, decimal coins)
        {
            var key = store.ExchangesId.FirstOrDefault();

            if (key == null) return;

            wallet.AddMoney(key, money);
            wallet.AddCoins(key, coins);
        }

        public void SetMoneyCoinsToLasExchange(ICryptoExchangeStore store, decimal money, decimal coins)
        {
            var key = store.ExchangesId.LastOrDefault();

            if (key == null) return;

            wallet.AddMoney(key, money);
            wallet.AddCoins(key, coins);
        }

        public void SetMoneyCoinsToRandomExchange(ICryptoExchangeStore store, decimal money, decimal coins)
        {
            Random random = new();
            int randomIndex = random.Next(store.ExchangesId.Count);
            var key = store.ExchangesId.ElementAt(randomIndex);
            wallet.AddMoney(key, money);
            wallet.AddCoins(key, coins);
        }

        public void SetMoneyCoinsToExchange(ICryptoExchangeStore store, string exchangeId, decimal money, decimal coins)
        {

            if (!store.ExchangesId.Contains(exchangeId))
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
            wallet = new Wallet();
            wallet.Add(money, coins);
        }
    }
}
