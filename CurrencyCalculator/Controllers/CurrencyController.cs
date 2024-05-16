using CurrencyCalculator.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyCalculator.Controllers
{
    // Controllers/CurrencyController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly IFrankfurterService _frankfurterService;

        public CurrencyController(IFrankfurterService frankfurterService)
        {
            _frankfurterService = frankfurterService;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestRates([FromQuery] string baseCurrency)
        {
            try
            {
                var result = await _frankfurterService.GetLatestRatesAsync(baseCurrency);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [HttpGet("convert")]
        public async Task<IActionResult> ConvertCurrency([FromQuery] string baseCurrency, [FromQuery] decimal amount, [FromQuery] string targetCurrency)
        {
            try
            {
                var result = await _frankfurterService.ConvertCurrencyAsync(baseCurrency, amount, targetCurrency);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [HttpGet("historical")]
        public async Task<IActionResult> GetHistoricalRates([FromQuery] string baseCurrency,[FromQuery] DateTime startDate,[FromQuery] DateTime endDate,[FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _frankfurterService.GetHistoricalRatesAsync(baseCurrency, startDate, endDate, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

    }

}
