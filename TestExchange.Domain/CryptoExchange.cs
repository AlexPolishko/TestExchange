namespace TestExchange.Domain
{
    public class CryptoExchange
    {
        public CryptoExchange(string id, OrderBook orderBook)
        {
            Id = id;
            OrderBook = orderBook;
        }

        public string Id { get; }
        public OrderBook OrderBook { get; }
    }
}
