using TestExchange.Domain;

namespace TestExchange.Application
{
    public class Resolver : IResolver
    {
        private readonly ICryptoExchangeStore store;
        private readonly PurchaseList purchaseList = new PurchaseList();
        private readonly Wallet wallet;
        private decimal remainingAmount = 0;

        public Resolver(ICryptoExchangeStore store)
        {
            this.store = store;
            wallet = new Wallet(store.Exchanges);
        }

        public PurchaseList Buy(decimal targetAmount)
        {
            remainingAmount = targetAmount;

            for (int index = 0; index < store.FlattenedAsks.Count; index++)
            {
                var currentAsk = store.FlattenedAsks[index];
                var purchase = new Order();

                if (wallet.EmptyWallet(currentAsk.ExchangeId))
                {
                    continue;
                }

                // Ask has more then enough 
                if (currentAsk.Amount > remainingAmount)
                {
                    // And we have enough money
                    if (currentAsk.Price * remainingAmount < wallet.Money(currentAsk.ExchangeId))
                    {
                        // We buy all and exit
                        purchase = Order.CreatePurchase(currentAsk, remainingAmount);
                    }
                    else
                    {
                        // Purchase using all of our available funds
                        purchase = Order.CreatePurchaseForAllMoney(currentAsk, wallet);
                    }

                }
                else
                {
                    if (currentAsk.TotalCost < wallet.Money(currentAsk.ExchangeId))
                    {
                        purchase = Order.CreatePurchase(currentAsk);
                    }
                    else
                    {
                        purchase = Order.CreatePurchaseForAllMoney(currentAsk, wallet);
                    }

                }

                Buy(purchase);

                if (remainingAmount == 0)
                    return purchaseList;
            }

            return purchaseList;
        }

        private void Buy(Order purchase)
        {
            remainingAmount -= purchase.Amount;
            purchaseList.AddPurchase(purchase);
            purchaseList.RemainingAmount = remainingAmount;
            wallet.PurchaseAll(purchase);
        }
    }
}
