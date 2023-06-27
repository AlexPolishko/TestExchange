namespace TestExchange.Domain
{
    public class СryptoExchange
    {
        public СryptoExchange(string id, OrderBook orderBook, decimal money, decimal amount)
        {
            Id = id;
            OrderBook = orderBook;
            Money = money;
            Amount = amount;
        }

        public string Id { get; }
        public OrderBook OrderBook { get; }
        public decimal Money { get; }
        public decimal Amount { get; }
    }
}
