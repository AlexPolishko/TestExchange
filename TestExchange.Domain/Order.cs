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
    }
}