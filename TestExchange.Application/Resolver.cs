using TestExchange.Domain;

namespace TestExchange.Application
{
    public class Resolver : IResolver
    {
        private readonly ICryptoExchangeStore store;
        private readonly Wallet wallet;

        public Resolver(ICryptoExchangeStore store)
        {
            this.store = store;
            wallet = new Wallet(store.Exchanges);
        }

        public PurchaseList Buy(decimal targetAmount)
        {
            var purchaseList = new PurchaseList(targetAmount);

            for (int index = 0; index < store.FlattenedAsks.Count; index++)
            {
                var currentAsk = store.FlattenedAsks[index];
                var purchase = new Order();

                if (wallet.EmptyWallet(currentAsk.ExchangeId))
                {
                    continue;
                }

                // Ask has more then enough 
                if (currentAsk.Amount > purchaseList.RemainingAmount)
                {
                    // And we have enough money
                    if (currentAsk.Price * purchaseList.RemainingAmount < wallet.Money(currentAsk.ExchangeId))
                    {
                        // We buy all and exit
                        purchase = Order.CreatePurchase(currentAsk, purchaseList.RemainingAmount);
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

                Buy(purchaseList, purchase);

                if (purchaseList.RemainingAmount == 0)
                    return purchaseList;
            }

            return purchaseList;
        }

        public PurchaseList Sell(decimal targetAmount)
        {
            var purchaseList = new PurchaseList(targetAmount);

            for (int index = 0; index < store.FlattenedBids.Count; index++)
            {
                var currentBids = store.FlattenedBids[index];
                var purchase = new Order();

                if (wallet.EmptyCoins(currentBids.ExchangeId))
                {
                    continue;
                }

                // Bid has more then enough 
                if (currentBids.Amount > purchaseList.RemainingAmount)
                {
                    purchase = Order.CreatePurchase(currentBids, purchaseList.RemainingAmount);
                }
                else
                {
                    purchase = Order.CreatePurchase(currentBids);
                }

                Sell(purchaseList, purchase);

                if (purchaseList.RemainingAmount == 0)
                    return purchaseList;

            }
        
            return purchaseList;

        }
        private void Buy(PurchaseList purchaseList, Order purchase)
        {
            purchaseList.AddPurchase(purchase);
            wallet.PurchaseAll(purchase);
        }

        private void Sell(PurchaseList purchaseList, Order purchase)
        {
            purchaseList.AddPurchase(purchase);
            wallet.SaleAll(purchase);
        }
    }
}
