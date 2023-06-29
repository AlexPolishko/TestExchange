using NSubstitute;
using TestExchange.Domain;

namespace TestExchange.Application.Tests
{
    public class ResolverTest
    {
        const string exchangeId1 = "1";
        const string exchangeId2 = "2";
        const string exchangeId3 = "3";

        [Fact]
        public void Buy_EnoughMoneyEnoughAmount_BuyOnlyOne()
        {
            //Arrange
            var amount = 0.05m;
            ICryptoExchangeStore store = CreateAsksWithAmount(1, 1, 1);
            var wallet = CreateWalletWithMoney(10m);

            store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null) }
                });

            var resolver = new Resolver(store, wallet);
            var expected = new PurchaseList(amount);
            expected.AddPurchase(new Order(1, amount, OrderType.Sell, exchangeId1));

            //Act
            var result = resolver.Buy(amount);

            //Assert
            Assert.True(expected.Items.SequenceEqual(result.Items));
            Assert.Equal(expected.RemainingAmount, result.RemainingAmount);
        }

        [Fact]
        public void Buy_EnoughMoneyLackingAmount_BuyThree()
        {
            //Arrange
            ICryptoExchangeStore store = CreateAsksWithAmount(1, 1, 1, 1);

            store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null) }
                });
            var wallet = CreateWalletWithMoney(10m);
            var resolver = new Resolver(store, wallet);
            var expected = new PurchaseList(0);
            expected.AddPurchase(new Order(1, 1, OrderType.Sell, exchangeId1));
            expected.AddPurchase(new Order(2, 1, OrderType.Sell, exchangeId1));
            expected.AddPurchase(new Order(3, 1, OrderType.Sell, exchangeId1));


            //Act
            var result = resolver.Buy(3);

            //Assert
            Assert.True(expected.Items.SequenceEqual(result.Items));
            Assert.Equal(0, result.RemainingAmount);
        }

        [Fact]
        public void Buy_LackingMoneyEnoughAmount_BuyTwo()
        {
            //Arrange
            var store = CreateAsksInDifferentExchangesWithAmount(10, 10, 10, 10);

            store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null) },
                    { exchangeId2 , new CryptoExchange(exchangeId2, null) }

                });
            var wallet = CreateWalletWithMoney(2m, 3m);
            var resolver = new Resolver(store, wallet);
            var expected = new PurchaseList(0);
            expected.AddPurchase(new Order(1, 2, OrderType.Sell, exchangeId1));
            expected.AddPurchase(new Order(3, 1, OrderType.Sell, exchangeId2));


            //Act
            var result = resolver.Buy(3);

            //Assert
            Assert.True(expected.Items.SequenceEqual(result.Items));
            Assert.Equal(0, result.RemainingAmount);
        }

        [Fact]
        public void Buy_LackingMoneyLackingAmount_BuyTwo()
        {
            //Arrange
            var store = CreateAsksInDifferentExchangesWithAmount(1, 1, 10, 10);

            store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null) },
                    { exchangeId2 , new CryptoExchange(exchangeId2, null) }

                });
            var wallet = CreateWalletWithMoney(2m, 5m);
            var resolver = new Resolver(store, wallet);
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

        [Fact]
        public void Buy_LackingMoneyLackingAmountFractualNumber_BuyTwo()
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
                    { exchangeId1 , new CryptoExchange(exchangeId1, null) },
                    { exchangeId2 , new CryptoExchange(exchangeId2, null) }

                });
            var wallet = CreateWalletWithMoney(0.08m, 3m);

            var resolver = new Resolver(store, wallet);
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


        [Fact]
        public void Buy_NotEnoughMoneyAtAll_ReturnRemainingAmountNotZero()
        {
            //Arrange
            var store = CreateAsksInDifferentExchangesWithAmount(10, 10, 10, 10);

            store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null) },
                    { exchangeId2 , new CryptoExchange(exchangeId2, null) }

                });

            var wallet = CreateWalletWithMoney(2m, 3m);

            var resolver = new Resolver(store, wallet);
            var expected = new PurchaseList(1);
            expected.AddPurchase(new Order(1, 2, OrderType.Sell, exchangeId1));
            expected.AddPurchase(new Order(3, 1, OrderType.Sell, exchangeId2));


            //Act
            var result = resolver.Buy(4);

            //Assert
            Assert.True(expected.Items.SequenceEqual(result.Items));
            Assert.Equal(1, result.RemainingAmount);
        }

        [Fact]
        public void Sell_EnoughAmount_SellOneBid()
        {
            //Arrange
            var amount = 0.05m;
            var store = Substitute.For<ICryptoExchangeStore>();
            store.FlattenedBids.Returns(new List<Order> {
                    new Order(1,5,OrderType.Sell,exchangeId1),
                    new Order(2,5,OrderType.Sell,exchangeId1),
                    new Order(3,5,OrderType.Sell,exchangeId1),
                });

            store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null) }
                });

            var wallet = CreateWalletWithCoins(10m);
            var resolver = new Resolver(store, wallet);
            var expected = new PurchaseList(0);
            expected.AddPurchase(new Order(1, amount, OrderType.Sell, exchangeId1));

            //Act
            var result = resolver.Sell(amount);

            //Assert
            Assert.True(expected.Items.SequenceEqual(result.Items));
            Assert.Equal(0, result.RemainingAmount);
        }

        [Fact]
        public void Sell_NotEnoughAmount_ReturnRemainingAmountNotZero()
        {
            //Arrange
            var amount = 100m;
            var store = Substitute.For<ICryptoExchangeStore>();
            store.FlattenedBids.Returns(new List<Order> {
                    new Order(1,5,OrderType.Sell,exchangeId1),
                    new Order(2,5,OrderType.Sell,exchangeId1),
                    new Order(3,5,OrderType.Sell,exchangeId1),
                });

            store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null) }
                });

            var wallet = CreateWalletWithCoins(10m);

            var resolver = new Resolver(store, wallet);
            var expected = new PurchaseList(90m);
            expected.AddPurchase(new Order(1, 5, OrderType.Sell, exchangeId1));
            expected.AddPurchase(new Order(2, 5, OrderType.Sell, exchangeId1));


            //Act
            var result = resolver.Sell(amount);

            //Assert
            Assert.True(expected.Items.SequenceEqual(result.Items));
            Assert.Equal(90m, result.RemainingAmount);
        }

        [Fact]
        public void Sell_LackAmount_SellTwoBids()
        {
            //Arrange
            var amount = 10m;
            var store = Substitute.For<ICryptoExchangeStore>();
            store.FlattenedBids.Returns(new List<Order> {
                    new Order(1,5,OrderType.Sell,exchangeId1),
                    new Order(2,5,OrderType.Sell,exchangeId1),
                    new Order(3,5,OrderType.Sell,exchangeId1),
                });

            store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null) }
                });

            var wallet = CreateWalletWithCoins(10m);

            var resolver = new Resolver(store, wallet);
            var expected = new PurchaseList(0);
            expected.AddPurchase(new Order(1, 5m, OrderType.Sell, exchangeId1));
            expected.AddPurchase(new Order(2, 5m, OrderType.Sell, exchangeId1));


            //Act
            var result = resolver.Sell(amount);

            //Assert
            Assert.True(expected.Items.SequenceEqual(result.Items));
            Assert.Equal(0, result.RemainingAmount);
        }

        [Fact]
        public void Sell_LackAmountInOneExchange_SellTwoBidsInTwoExchange()
        {
            //Arrange
            var amount = 10m;
            var store = Substitute.For<ICryptoExchangeStore>();
            store.FlattenedBids.Returns(new List<Order> {
                    new Order(1,5,OrderType.Sell,exchangeId1),
                    new Order(2,50,OrderType.Sell,exchangeId2),
                    new Order(3,5,OrderType.Sell,exchangeId3),
                });

            store.Exchanges.Returns(new Dictionary<string, CryptoExchange>
                {
                    { exchangeId1 , new CryptoExchange(exchangeId1, null) },
                    { exchangeId2 , new CryptoExchange(exchangeId2, null) }
                });

            var wallet = CreateWalletWithCoins(10m, 10m);
            var resolver = new Resolver(store, wallet);
            var expected = new PurchaseList(0);
            expected.AddPurchase(new Order(1, 5m, OrderType.Sell, exchangeId1));
            expected.AddPurchase(new Order(2, 5m, OrderType.Sell, exchangeId2));


            //Act
            var result = resolver.Sell(amount);

            //Assert
            Assert.True(expected.Items.SequenceEqual(result.Items));
            Assert.Equal(0, result.RemainingAmount);
        }


        private static ICryptoExchangeStore CreateAsksWithAmount(params decimal[] amount)
        {
            var list = new List<Order>();
            for (int i = 0; i < amount.Length; i++)
            {
                list.Add(new Order(i + 1, amount[i], OrderType.Sell, exchangeId1));
            }

            var store = Substitute.For<ICryptoExchangeStore>();
            store.FlattenedAsks.Returns(list);
            return store;
        }

        private static ICryptoExchangeStore CreateAsksInDifferentExchangesWithAmount(params decimal[] amount)
        {
            var list = new List<Order>();
            for (int i = 0; i < amount.Length; i++)
            {
                list.Add(new Order(i + 1, amount[i], OrderType.Sell, i < 2 ? exchangeId1 : exchangeId2));
            }

            var store = Substitute.For<ICryptoExchangeStore>();
            store.FlattenedAsks.Returns(list);
            return store;
        }

        private static WalletService CreateWalletWithMoney(params decimal[] money)
        {
            var moneyDictionary = new Dictionary<string, decimal>();

            for (int i = 0; i < money.Length; i++)
            {
                if (i == 0) moneyDictionary.Add(exchangeId1, money[i]);
                if (i == 1) moneyDictionary.Add(exchangeId2, money[i]);
                if (i == 2) moneyDictionary.Add(exchangeId3, money[i]);
            }
            WalletService walletService = new WalletService();
            walletService.CreateWallet(moneyDictionary, new Dictionary<string, decimal>());

            return walletService;
        }

        private static WalletService CreateWalletWithCoins(params decimal[] coins)
        {
            var coinsDictionary = new Dictionary<string, decimal>();

            for (int i = 0; i < coins.Length; i++)
            {
                if (i == 0) coinsDictionary.Add(exchangeId1, coins[i]);
                if (i == 1) coinsDictionary.Add(exchangeId2, coins[i]);
                if (i == 2) coinsDictionary.Add(exchangeId3, coins[i]);
            }
            WalletService walletService = new WalletService();
            walletService.CreateWallet(new Dictionary<string, decimal>(), coinsDictionary);

            return walletService;
        }

    }
}
