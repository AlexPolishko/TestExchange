using TestExchange.Domain;

namespace TestExchange.Application
{
    public class Resolver : IResolver
    {
        private readonly ICryptoExchangeStore store;
        private readonly IWalletService walletService;

        public Resolver(ICryptoExchangeStore store, IWalletService walletService)
        {
            this.store = store;
            this.walletService = walletService;
        }

        public PurchaseList Buy(decimal targetAmount)
        {
            var wallet = walletService.GetWallet();
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

                Buy(purchaseList, purchase, wallet);

                if (purchaseList.RemainingAmount == 0)
                    return purchaseList;
            }

            return purchaseList;
        }

        public PurchaseList Sell(decimal targetAmount)
        {
            var purchaseList = new PurchaseList(targetAmount);
            var wallet = walletService.GetWallet();

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
                    if (purchaseList.RemainingAmount > wallet.Coins(currentBids.ExchangeId))
                    {
                        purchase = Order.CreatePurchase(currentBids, wallet.Coins(currentBids.ExchangeId));
                    }
                    else
                    {
                        purchase = Order.CreatePurchase(currentBids, purchaseList.RemainingAmount);
                    }
                }
                else
                {
                    if (currentBids.Amount > wallet.Coins(currentBids.ExchangeId))
                    {
                        purchase = Order.CreatePurchase(currentBids, wallet.Coins(currentBids.ExchangeId));
                    }
                    else
                    {
                        purchase = Order.CreatePurchase(currentBids);
                    }
                }

                Sell(purchaseList, purchase, wallet);

                if (purchaseList.RemainingAmount == 0)
                    return purchaseList;

            }

            return purchaseList;

        }
        private void Buy(PurchaseList purchaseList, Order purchase, Wallet wallet)
        {
            purchaseList.AddPurchase(purchase);
            wallet.Purchase(purchase);
        }

        private void Sell(PurchaseList purchaseList, Order purchase, Wallet wallet)
        {
            purchaseList.AddPurchase(purchase);
            wallet.Sale(purchase);
        }
    }
}
