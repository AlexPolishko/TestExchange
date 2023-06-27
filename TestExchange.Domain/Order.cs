namespace TestExchange.Domain
{
    public struct Order
    {
        public Order(decimal price, decimal amount, OrderType orderType, string exchangeId)
        {
            Price = price;
            Amount = amount;
            OrderType = orderType;
            ExchangeId = exchangeId;
        }

        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public OrderType OrderType { get; set; }

        public string ExchangeId { get; set; }

        public decimal TotalCost
        {
            get { return Price * Amount; }
        }

        public static Order CreatePurchase(Order order, decimal? amount = null)
        {
            if (amount == null)
                amount = order.Amount;

            return new Order(order.Price, amount.Value, order.OrderType, order.ExchangeId);
        }

        public static Order CreatePurchaseForAllMoney(Order order, Wallet wallet)
        {
            var amount = Decimal.Divide(wallet.Money(order.ExchangeId), order.Price);
            return Order.CreatePurchase(order, amount);
        }
    }
}