namespace TestExchange.Domain
{
    public class Wallet
    {
        private Dictionary<string, Decimal> money = new Dictionary<string, Decimal>();
        private Dictionary<string, Decimal> coins = new Dictionary<string, Decimal>();

        public Wallet(Dictionary<string, CryptoExchange> exchanges)
        {
            foreach (var exchange in exchanges)
            {
                money.Add(exchange.Key, exchange.Value.Money);
                coins.Add(exchange.Key, exchange.Value.Amount);
            }
        }

        public void SaleAll(Order order)
        {
            Sale(order.ExchangeId, order.Amount);
        }

        public void PurchaseAll(Order order)
        {
            Purchase(order.ExchangeId, order.TotalCost);
        }

        public decimal Money(string exchangeId)
        {
            return money[exchangeId];
        }

        public decimal Coins(string exchangeId)
        {
            return coins[exchangeId];
        }

        public bool EmptyWallet(string exchangeId)
        {
            if (!money.ContainsKey(exchangeId)) return true;

            return (Math.Round(money[exchangeId], 2) == 0);
        }

        public bool EmptyCoins(string exchangeId)
        {
            if (!coins.ContainsKey(exchangeId)) return true;

            return (Math.Round(coins[exchangeId], 5) == 0);
        }

        private void Purchase(string exchangeId, decimal money)
        {
            if (!this.money.Keys.Contains(exchangeId))
                throw new ApplicationException($"Invalid exchangeId:{exchangeId}");

            if (Math.Round(this.money[exchangeId] - money, 2) < 0)
                throw new ApplicationException($"insufficient funds:{money} less then wallet has {this.money[exchangeId]}");

            this.money[exchangeId] -= money;
        }

        private void Sale(string exchangeId, decimal amount)
        {
            if (!this.coins.Keys.Contains(exchangeId))
                throw new ApplicationException($"Invalid exchangeId:{exchangeId}");

            if (Math.Round(this.coins[exchangeId] - amount, 5) < 0)
                throw new ApplicationException($"insufficient coins:{amount} less then wallet has {this.coins[exchangeId]}");

            this.coins[exchangeId] -= amount;
        }
    }
}
