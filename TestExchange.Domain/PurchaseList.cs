namespace TestExchange.Domain
{
    public class PurchaseList
    {
        public List<Order> Items { get; } = new List<Order>();
        public decimal RemainingAmount { get; set; }
        public void AddPurchase(Order purchase)
        {
            Items.Add(purchase);
        }
    }
}
