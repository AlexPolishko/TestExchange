namespace TestExchange.Domain
{
    public class PurchaseList
    {
        public List<Order> Items { get; }
        public void AddPurchase(Order purchase)
        {
            Items.Add(purchase);
        }
    }
}
