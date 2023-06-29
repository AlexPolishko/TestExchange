using Microsoft.AspNetCore.Mvc;
using TestExchange.API.Models;
using TestExchange.Application;
using TestExchange.Domain;

namespace TestExchange.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WalletController : ControllerBase
    {
        private readonly ILogger<WalletController> _logger;
        private readonly IWalletService _walletService;
        private readonly ICryptoExchangeStore _store;

        public WalletController(ILogger<WalletController> logger, IWalletService walletService, ICryptoExchangeStore store)
        {
            _logger = logger;
            _walletService = walletService;
            _store = store;
        }

        /// <summary>
        /// Return JSON with all your exchange's balance
        /// </summary>
        /// <returns>The balance and coin</returns>
        [HttpGet]
        public IActionResult Get()
        {
            var wallet = _walletService.GetWallet();
            return Ok(wallet.ToString());
        }

        /// <summary>
        /// Add the same amount of money to all wallets
        /// </summary>
        [HttpPost("AddMoney")]
        public IActionResult AddMoneyToAllExchanges([FromBody] decimal money)
        {
            if (money < 0)
                return BadRequest("Money amount should be nonnegative");

            _walletService.AddToAllExchange(_store, money, 0m);
            return Ok();
        }

        /// <summary>
        /// Add the same amount of coins to all wallets
        /// </summary>
        [HttpPost("AddCoins")]
        public IActionResult AddCoinsToAllExchanges([FromBody] decimal amount)
        {
            if (amount < 0)
                return BadRequest("Coins amount should be nonnegative");

            _walletService.AddToAllExchange(_store, 0m, amount);
            return Ok();
        }


        /// <summary>
        /// Add money or coins to specific exchange balance
        /// </summary>
        /// <param name="model">The model with exchangeId, money and coins.</param>
        [HttpPost("Exchange")]
        public IActionResult AddToExchanges([FromBody] WalletRequestModel model)
        {
            if (model == null || model.Coins < 0 || model.Money < 0)
                return BadRequest();

            _walletService.ChangeExchange(_store, model.ExchangeId, model.Money, model.Coins);
            return Ok();
        }
    }
}