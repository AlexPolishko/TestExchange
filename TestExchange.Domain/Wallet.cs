namespace TestExchange.Domain
{
    public class Wallet
    {
        private Dictionary<string, Decimal> _wallets = new Dictionary<string, Decimal>();

        public  Wallet(Dictionary<string, CryptoExchange> exchanges)
        {
            foreach (var exchange in exchanges)
            {
                _wallets.Add(exchange.Key, exchange.Value.Money);
            }
        }

        public void Purchase(string exchangeId, decimal money)
        {
            if (!_wallets.Keys.Contains(exchangeId))
                throw new ApplicationException($"Invalid exchangeId:{exchangeId}");

            if (Math.Round(_wallets[exchangeId] - money,2) < 0)
                throw new ApplicationException($"insufficient funds:{money} less then wallet has {_wallets[exchangeId]}");

            _wallets[exchangeId] -= money;
        }

        public void PurchaseAll(Order order)
        {
            Purchase(order.ExchangeId, order.TotalCost);
        }

        public decimal Money(string exchangeId)
        {
            return _wallets[exchangeId];
        }

        public bool EmptyWallet(string exchangeId)
        {
            if (!_wallets.ContainsKey(exchangeId)) return true;

            return (Math.Round(_wallets[exchangeId], 2) == 0);
        }
    }
}
