namespace TestExchange.Domain
{
    public struct Order
    {
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public OrderType OrderType { get; set; }
    }
}