namespace TestExchange.Domain
{
    public class PurchaseList
    {
        public List<Order> Items { get; } = new List<Order>();
        public void AddPurchase(Order purchase)
        {
            Items.Add(purchase);
        }
    }
}
