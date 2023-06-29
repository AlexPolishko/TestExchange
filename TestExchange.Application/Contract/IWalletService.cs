using TestExchange.Domain;

namespace TestExchange.Application
{
    public interface IWalletService
    {
        void AddToAllExchange(ICryptoExchangeStore store, decimal money, decimal coins);
        void ChangeExchange(ICryptoExchangeStore store, string exchangeId, decimal money, decimal coins);
        void ChangeFirstExchange(ICryptoExchangeStore store, decimal money, decimal coins);
        void ChangeLastExchange(ICryptoExchangeStore store, decimal money, decimal coins);
        void ChangeRandomExchange(ICryptoExchangeStore store, decimal money, decimal coins);
        Wallet GetWallet();
    }
}