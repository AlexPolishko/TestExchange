namespace TestExchange.Domain
{
    public class PurchaseList
    {
        public List<Order> Items { get; } = new List<Order>();
        public decimal RemainingAmount { get; private set; }

        public PurchaseList(decimal remainingAmount)
        {
            RemainingAmount = remainingAmount;
        }

        public void AddPurchase(Order purchase)
        {
            Items.Add(purchase);
            RemainingAmount -= purchase.Amount;
        }

        public bool IsPurchaseSuccessful()
        {
            return RemainingAmount == 0;
        }
    }
}
