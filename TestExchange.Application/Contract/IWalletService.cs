using TestExchange.Domain;

namespace TestExchange.Application
{
    public interface IWalletService
    {
        void SetMoneyToAllExchanges(ICryptoExchangeStore store, decimal money);
        void SetCoinsToAllExchanges(ICryptoExchangeStore store, decimal coins);
        void SetMoneyCoinsToExchange(ICryptoExchangeStore store, string exchangeId, decimal money, decimal coins);
        void SetMoneyCoinsToFirstExchange(ICryptoExchangeStore store, decimal money, decimal coins);
        void SetMoneyCoinsToLasExchange(ICryptoExchangeStore store, decimal money, decimal coins);
        void SetMoneyCoinsToRandomExchange(ICryptoExchangeStore store, decimal money, decimal coins);
        Wallet GetWallet();
    }
}