namespace TestExchange.API.Models
{
    public class WalletRequestModel
    {
        public string ExchangeId { get; set; }
        public decimal Money { get; set; }
        public decimal Coins { get; set; }
    }
}
