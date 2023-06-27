using TestExchange.Domain;

namespace TestExchange.Application
{
    public class Resolver
    {
        private readonly OrderBook orderBook;

        public Resolver(OrderBook orderBook)
        {
            this.orderBook = orderBook;
        }

        public PurchaseList Buy(decimal balance)
        {
            var purchaseList = new PurchaseList();
            var sortedAsks = orderBook.Asks.OrderBy(x => x.Price).ToArray();
            var index = 0;

            while (balance>0)
            {
                if (balance > sortedAsks[index].TotalCost)
                {
                    balance -= sortedAsks[index].TotalCost;
                    Console.WriteLine($"Ask with price:{sortedAsks[index].Price} was bought with amount {sortedAsks[index].Amount}. Remain balance {balance}");
                    purchaseList.AddPurchase(sortedAsks[index]);
                    index++;
                }
                else
                {
                    var amount = balance / sortedAsks[index].Price;
                    Console.WriteLine($"Ask with price:{sortedAsks[index].Price} was bought with amount {amount} of {sortedAsks[index].Amount}. purchases have been finalized.");
                    purchaseList.AddPurchase(new Order(sortedAsks[index].Price, amount, OrderType.Buy);
                    balance = 0;
                }
            }

            return purchaseList;
        }
    }
}
