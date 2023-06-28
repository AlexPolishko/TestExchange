namespace TestExchange.Domain
{
    public class Wallet
    {
        private Dictionary<string, Decimal> money = new Dictionary<string, Decimal>();
        private Dictionary<string, Decimal> coins = new Dictionary<string, Decimal>();

        public Wallet()
        {
        }

        public Wallet(Dictionary<string, Decimal> money, Dictionary<string, Decimal> coins)
        {
            this.money = money;
            this.coins = coins;
        }

        public void Sale(Order order)
        {
            Sale(order.ExchangeId, order.Amount);
        }

        public void Purchase(Order order)
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

        public void AddMoney(string exchangeId, decimal money)
        {
            this.money[exchangeId] = money;
        }

        public void AddCoins(string exchangeId, decimal amount)
        {
            this.coins[exchangeId] = amount;
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
