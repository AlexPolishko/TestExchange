﻿using NSubstitute;
using TestExchange.Domain;

namespace TestExchange.Application.Tests
{
    public class ResolverTest
    {
        const string exchangeId1 = "1";
        const string exchangeId2 = "2";
        const string exchangeId3 = "3";

        const decimal BestPrice = 1.05m;


        [Fact]
        public void Buy_EnoughMoneyEnoughAmount_BuyOnlyOne()
        {
            {
                //Arrange
                var amount = 0.05m;
                var store = Substitute.For<ICryptoExchangeStore>();
                store.FlattenedAsks.Returns(new List<Order> {
                    new Order(BestPrice,5,OrderType.Sell,exchangeId1),
                    new Order(2,5,OrderType.Sell,exchangeId1),
                    new Order(3,5,OrderType.Sell,exchangeId1),
                });

                store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null, 10, 0) }
                });

                var resolver = new Resolver(store);
                var expected = new PurchaseList(amount);
                expected.AddPurchase(new Order(BestPrice, amount, OrderType.Sell, exchangeId1));

                //Act
                var result = resolver.Buy(amount);

                //Assert
                Assert.True(expected.Items.SequenceEqual(result.Items));
                Assert.Equal(expected.RemainingAmount, result.RemainingAmount);
            }
        }

        [Fact]
        public void Buy_EnoughMoneyLackingAmount_BuyThree()
        {
            {
                //Arrange
                var store = Substitute.For<ICryptoExchangeStore>();
                store.FlattenedAsks.Returns(new List<Order> {
                    new Order(BestPrice,1,OrderType.Sell,exchangeId1),
                    new Order(2,1,OrderType.Sell,exchangeId1),
                    new Order(3,1,OrderType.Sell,exchangeId1),
                    new Order(4,1,OrderType.Sell,exchangeId1),
                });

                store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null, 10, 0) }
                });

                var resolver = new Resolver(store);
                var expected = new PurchaseList(0);
                expected.AddPurchase(new Order(BestPrice, 1, OrderType.Sell, exchangeId1));
                expected.AddPurchase(new Order(2, 1, OrderType.Sell, exchangeId1));
                expected.AddPurchase(new Order(3, 1, OrderType.Sell, exchangeId1));


                //Act
                var result = resolver.Buy(3);

                //Assert
                Assert.True(expected.Items.SequenceEqual(result.Items));
                Assert.Equal(0, result.RemainingAmount);
            }
        }

        [Fact]
        public void Buy_LackingMoneyEnoughAmount_BuyTwo()
        {
            {
                //Arrange
                var store = Substitute.For<ICryptoExchangeStore>();
                store.FlattenedAsks.Returns(new List<Order> {
                    new Order(1,10,OrderType.Sell,exchangeId1),
                    new Order(2,10,OrderType.Sell,exchangeId1),
                    new Order(3,10,OrderType.Sell,exchangeId2),
                    new Order(4,10,OrderType.Sell,exchangeId2),
                });

                store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null, 2, 0) },
                    { exchangeId2 , new CryptoExchange(exchangeId2, null, 3, 0) }

                });

                var resolver = new Resolver(store);
                var expected = new PurchaseList(0);
                expected.AddPurchase(new Order(1, 2, OrderType.Sell, exchangeId1));
                expected.AddPurchase(new Order(3, 1, OrderType.Sell, exchangeId2));


                //Act
                var result = resolver.Buy(3);

                //Assert
                Assert.True(expected.Items.SequenceEqual(result.Items));
                Assert.Equal(0, result.RemainingAmount);
            }
        }

        [Fact]
        public void Buy_LackingMoneyLackingAmount_BuyTwo()
        {
            {
                //Arrange
                var store = Substitute.For<ICryptoExchangeStore>();
                store.FlattenedAsks.Returns(new List<Order> {
                    new Order(1,1,OrderType.Sell,exchangeId1),
                    new Order(2,1,OrderType.Sell,exchangeId1),
                    new Order(3,10,OrderType.Sell,exchangeId2),
                    new Order(4,10,OrderType.Sell,exchangeId2),
                });

                store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null, 2, 0) },
                    { exchangeId2 , new CryptoExchange(exchangeId2, null, 5, 0) }

                });

                var resolver = new Resolver(store);
                var expected = new PurchaseList(0);
                expected.AddPurchase(new Order(1, 1, OrderType.Sell, exchangeId1));
                expected.AddPurchase(new Order(2, 0.5m, OrderType.Sell, exchangeId1));
                expected.AddPurchase(new Order(3, 1.5m, OrderType.Sell, exchangeId2));


                //Act
                var result = resolver.Buy(3);

                //Assert
                Assert.True(expected.Items.SequenceEqual(result.Items));
                Assert.Equal(0, result.RemainingAmount);
            }
        }

        [Fact]
        public void Buy_LackingMoneyLackingAmountFractualNumber_BuyTwo()
        {
            {
                //Arrange
                var store = Substitute.For<ICryptoExchangeStore>();
                store.FlattenedAsks.Returns(new List<Order> {
                    new Order(1.05m, 0.05m,OrderType.Sell,exchangeId1),
                    new Order(1.10m, 0.1m,OrderType.Sell,exchangeId1),
                    new Order(1.11m, 0.005m,OrderType.Sell,exchangeId2),
                    new Order(1.2m,10,OrderType.Sell,exchangeId2),
                });

                store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null, 0.08m, 0) },
                    { exchangeId2 , new CryptoExchange(exchangeId2, null, 3m, 0) }

                });

                var resolver = new Resolver(store);
                var expected = new PurchaseList(0);
                expected.AddPurchase(new Order(1.05m, 0.05m, OrderType.Sell, exchangeId1));
                expected.AddPurchase(new Order(1.10m, 0.025m, OrderType.Sell, exchangeId1));
                expected.AddPurchase(new Order(1.11m, 0.005m, OrderType.Sell, exchangeId2));
                expected.AddPurchase(new Order(1.2m, 0.270m, OrderType.Sell, exchangeId2));


                //Act
                var result = resolver.Buy(0.35m);

                //Assert
                Assert.True(expected.Items.SequenceEqual(result.Items));
                Assert.Equal(0, result.RemainingAmount);
            }
        }


        [Fact]
        public void Buy_NotEnoughMoneyAtAll_ReturnRemainingAmountNotZero()
        {
            {
                //Arrange
                var store = Substitute.For<ICryptoExchangeStore>();
                store.FlattenedAsks.Returns(new List<Order> {
                    new Order(1,10,OrderType.Sell,exchangeId1),
                    new Order(2,10,OrderType.Sell,exchangeId1),
                    new Order(3,10,OrderType.Sell,exchangeId2),
                    new Order(4,10,OrderType.Sell,exchangeId3),
                });

                store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null, 2, 0) },
                    { exchangeId2 , new CryptoExchange(exchangeId2, null, 3, 0) }

                });

                var resolver = new Resolver(store);
                var expected = new PurchaseList(1);
                expected.AddPurchase(new Order(1, 2, OrderType.Sell, exchangeId1));
                expected.AddPurchase(new Order(3, 1, OrderType.Sell, exchangeId2));


                //Act
                var result = resolver.Buy(4);

                //Assert
                Assert.True(expected.Items.SequenceEqual(result.Items));
                Assert.Equal(1, result.RemainingAmount);
            }
        }

        [Fact]
        public void Sell_EnoughAmount_BuyOnlyOne()
        {
            {
                //Arrange
                var amount = 0.05m;
                var store = Substitute.For<ICryptoExchangeStore>();
                store.FlattenedBids.Returns(new List<Order> {
                    new Order(BestPrice,5,OrderType.Sell,exchangeId1),
                    new Order(2,5,OrderType.Sell,exchangeId1),
                    new Order(3,5,OrderType.Sell,exchangeId1),
                });

                store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null, 0, 10) }
                });

                var resolver = new Resolver(store);
                var expected = new PurchaseList(0);
                expected.AddPurchase(new Order(BestPrice, amount, OrderType.Sell, exchangeId1));

                //Act
                var result = resolver.Sell(amount);

                //Assert
                Assert.True(expected.Items.SequenceEqual(result.Items));
                Assert.Equal(0, result.RemainingAmount);
            }
        }

        [Fact]
        public void Sell_NotEnoughAmount_ReturnRemainingAmountNotZero()
        {
            {
                //Arrange
                var amount = 100m;
                var store = Substitute.For<ICryptoExchangeStore>();
                store.FlattenedBids.Returns(new List<Order> {
                    new Order(BestPrice,5,OrderType.Sell,exchangeId1),
                    new Order(2,5,OrderType.Sell,exchangeId1),
                    new Order(3,5,OrderType.Sell,exchangeId1),
                });

                store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null, 0, 10) }
                });

                var resolver = new Resolver(store);
                var expected = new PurchaseList(90m);
                expected.AddPurchase(new Order(BestPrice, 5, OrderType.Sell, exchangeId1));
                expected.AddPurchase(new Order(2, 5, OrderType.Sell, exchangeId1));


                //Act
                var result = resolver.Sell(amount);

                //Assert
                Assert.True(expected.Items.SequenceEqual(result.Items));
                Assert.Equal(90m, result.RemainingAmount);
            }
        }


    }
}
