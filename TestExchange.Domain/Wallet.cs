using System.Text.Json;

namespace TestExchange.Domain
{
    public class Wallet
    {
        private Dictionary<string, Decimal> money = new();
        private Dictionary<string, Decimal> coins = new();

        public Wallet()
        {
        }

        public void Add(Dictionary<string, Decimal>? money, Dictionary<string, Decimal>? coins)
        {
            if (money != null) this.money = money;
            if (coins != null) this.coins = coins;
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

        public override string ToString()
        {
            return JsonSerializer.Serialize(new { money, coins });
        }

        public Wallet Clone()
        {
            var clone = new Wallet();
            clone.Add
            (
                new Dictionary<string, decimal>(this.money),
                new Dictionary<string, decimal>(this.coins)
            );

            return clone;
        }

        private void Purchase(string exchangeId, decimal money)
        {
            if (!this.money.ContainsKey(exchangeId))
                throw new ArgumentException($"Invalid exchangeId:{exchangeId}");

            if (Math.Round(this.money[exchangeId] - money, 2) < 0)
                throw new ArgumentException($"insufficient funds:{money} less then wallet has {this.money[exchangeId]}");

            this.money[exchangeId] -= money;
        }

        private void Sale(string exchangeId, decimal amount)
        {
            if (!this.coins.ContainsKey(exchangeId))
                throw new ArgumentException($"Invalid exchangeId:{exchangeId}");

            if (Math.Round(this.coins[exchangeId] - amount, 5) < 0)
                throw new ArgumentException($"insufficient coins:{amount} less then wallet has {this.coins[exchangeId]}");

            this.coins[exchangeId] -= amount;
        }
    }
}
