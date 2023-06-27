using TestExchange.Domain;

namespace TestExchange.Application
{
    public class Resolver
    {
        private readonly CryptoExchangeStore store;

        public Resolver(CryptoExchangeStore store)
        {
            this.store = store;
        }

        public PurchaseList Buy(decimal targetAmount)
        {
            var purchaseList = new PurchaseList();
            var index = 0;
            var wallet = new Dictionary<string, decimal>();
            var remainingAmount = targetAmount;

            foreach (var exahange in store.Exchanges)
            {
                wallet.Add(exahange.Key, exahange.Value.Money);
            }

            while (remainingAmount > 0 || index> store.FlattenedAsks.Count)
            {
                var currentAsk = store.FlattenedAsks[index];

                // Ask has more then enough 
                if (currentAsk.Amount > remainingAmount)
                {
                    // And we have enough money
                    if (currentAsk.Price * remainingAmount > wallet[currentAsk.ExchangeId])
                    {
                        // We buy all and exit
                        var purchase = new Order(currentAsk.Price, remainingAmount, OrderType.Buy, currentAsk.ExchangeId);
                        purchaseList.AddPurchase(purchase);
                        return purchaseList;
                    }
                    else
                    {
                        // Purchase using all of our available funds
                        var amount = wallet[currentAsk.ExchangeId] / currentAsk.Price;
                        wallet[currentAsk.ExchangeId] = 0;
                        remainingAmount -= amount;
                        var purchase = new Order(currentAsk.Price, amount, OrderType.Buy, currentAsk.ExchangeId);
                        purchaseList.AddPurchase(purchase);
                    }

                }
                else
                {
                    // Ask has not enough amount
                    if (currentAsk.TotalCost < wallet[currentAsk.ExchangeId])
                    {
                        wallet[currentAsk.ExchangeId] -= currentAsk.TotalCost;
                        remainingAmount -= currentAsk.Amount;
                        var purchase = new Order(currentAsk.Price, currentAsk.Amount, OrderType.Buy, currentAsk.ExchangeId);
                        purchaseList.AddPurchase(purchase);
                    }
                    else
                    {
                        // Purchase using all of our available funds
                        var amount = wallet[currentAsk.ExchangeId] / currentAsk.Price;
                        wallet[currentAsk.ExchangeId] = 0;
                        remainingAmount -= amount;
                        var purchase = new Order(currentAsk.Price, amount, OrderType.Buy, currentAsk.ExchangeId);
                        purchaseList.AddPurchase(purchase);
                    }
                }

                index++;
            }

            return purchaseList;
        }
    }
}
