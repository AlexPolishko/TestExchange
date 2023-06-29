using Microsoft.AspNetCore.Mvc;
using TestExchange.Application;

namespace TestExchange.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResolverController : ControllerBase
    {
        private readonly IResolver resolver;

        public ResolverController(IResolver resolver)
        {
            this.resolver = resolver;
        }

        /// <summary>
        /// Return JSON response with the "best execution" plan with min price.
        /// </summary>
        /// <returns>List of purchases</returns>
        [HttpPost("Buy")]
        public IActionResult Buy([FromBody]decimal targetAmount)
        {
            if (targetAmount < 0)
                return BadRequest("targetAmount should be nonnegative");

            var result = resolver.Buy(targetAmount);
            return Ok(result);
        }

        /// <summary>
        /// Return JSON response with the "best execution" plan with max price.
        /// </summary>
        /// <returns>List of purchases</returns>
        [HttpPost("Sell")]
        public IActionResult Sell([FromBody] decimal targetAmount)
        {
            if (targetAmount < 0)
                return BadRequest("targetAmount should be nonnegative");

            var result = resolver.Sell(targetAmount);
            return Ok(result);
        }
    }
}
