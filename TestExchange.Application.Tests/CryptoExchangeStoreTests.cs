using NSubstitute;
using TestExchange.Domain;

namespace TestExchange.Application.Tests
{
    public class CryptoExchangeStoreTests
    {
        const string IdExchange1 = "Id1";
        const string IdExchange2 = "Id2";

        [Fact]
        public void CryptoExchangeStore_FulFillExchanges_HasValue()
        {
            // Arrange
            OrderBook orderBook = CreateTestOrderBookWithAsk(IdExchange1);

            Dictionary<string, OrderBook> dictionary = new Dictionary<string, OrderBook>{
                {IdExchange1, orderBook }, 
                {IdExchange2, orderBook } };
            IOrderBookReader reader = Substitute.For<IOrderBookReader>();
            reader.Read().Returns(dictionary);

            // Act
            var store = new CryptoExchangeStore(reader);

            // Assert
            Assert.Equal(2, store.Exchanges.Count);
        }

        [Fact]
        public void CryptoExchangeStore_FulFillExchanges_FlattenedAsksOrderAsc()
        {
            // Arrange
            var orderBook1 = CreateTestOrderBookWithAsk(IdExchange1, 5, 1, 3);
            var orderBook2 = CreateTestOrderBookWithAsk(IdExchange2, 2, 4);

            var expected = new Order[] { orderBook1.Asks[1], orderBook2.Asks[0], orderBook1.Asks[2], orderBook2.Asks[1], orderBook1.Asks[0] };

            Dictionary<string, OrderBook> dictionary = new Dictionary<string, OrderBook>{
                {IdExchange1, orderBook1 },
                {IdExchange2, orderBook2 } };
            IOrderBookReader reader = Substitute.For<IOrderBookReader>();
            reader.Read().Returns(dictionary);

            // Act
            var store = new CryptoExchangeStore(reader);

            // Assert
            Assert.True(expected.SequenceEqual(store.FlattenedAsks));
        }

        [Fact]
        public void CryptoExchangeStore_FulFillExchanges_FlattenedBidsOrderDesc()
        {
            // Arrange
            var orderBook1 = CreateTestOrderBookWithBids(IdExchange1, 5, 1, 3);
            var orderBook2 = CreateTestOrderBookWithBids(IdExchange2, 2, 4);

            var expected = new Order[] { orderBook1.Bids[0], orderBook2.Bids[1], orderBook1.Bids[2], orderBook2.Bids[0], orderBook1.Bids[1] };

            Dictionary<string, OrderBook> dictionary = new Dictionary<string, OrderBook>{
                {IdExchange1, orderBook1 },
                {IdExchange2, orderBook2 } };
            IOrderBookReader reader = Substitute.For<IOrderBookReader>();
            reader.Read().Returns(dictionary);

            // Act
            var store = new CryptoExchangeStore(reader);

            // Assert
            Assert.True(expected.SequenceEqual(store.FlattenedBids));
        }

        private static OrderBook CreateTestOrderBookWithAsk(string exchangeId, params int[] askPrices)
        {
            var orderBook = new OrderBook();
            orderBook.Asks = new Order[askPrices.Length];
            orderBook.Bids = new Order[0];

            for (int i = 0; i < askPrices.Length; i++)
                orderBook.Asks[i] = new Order(askPrices[i], 1, OrderType.Sell, exchangeId);

            return orderBook;
        }

        private static OrderBook CreateTestOrderBookWithBids(string exchangeId, params int[] bidPrices)
        {
            var orderBook = new OrderBook();
            orderBook.Asks = new Order[0];
            orderBook.Bids = new Order[bidPrices.Length];

            for (int i = 0; i < bidPrices.Length; i++)
                orderBook.Bids[i] = new Order(bidPrices[i], 1, OrderType.Sell, exchangeId);

            return orderBook;
        }
    }
}