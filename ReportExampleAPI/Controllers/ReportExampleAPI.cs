using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ReportExampleAPI.Models;

namespace ReportExampleAPI.Controllers
{
    
    [ApiController]
    [Route("api/stocks")]
    public class ReportExampleAPI : ControllerBase
    {
        private static readonly Random _random = new Random();

        [HttpGet("{symbol}")]
        public IActionResult GetStockData(string symbol)
        {
            if (string.IsNullOrEmpty(symbol))
            {
                return BadRequest("Il simbolo del titolo non può essere vuoto.");
            }

            var stockData = GenerateStockPrice(symbol.ToUpper());
            return Ok(stockData);
        }


        private StockPrice GenerateStockPrice(string symbol)
        {
            decimal yesterdayPrice = 100m + ((decimal)(_random.NextDouble() * 20 - 10)); // Prezzo casuale tra 90 e 110
            decimal todayPrice = yesterdayPrice + ((decimal)(_random.NextDouble() * 4 - 2)); // Variazione tra -2 e +2
            todayPrice = Math.Round(todayPrice, 2);
            yesterdayPrice = Math.Round(yesterdayPrice, 2);

            decimal percentageChange = Math.Round(((todayPrice - yesterdayPrice) / yesterdayPrice) * 100, 2);

            return new StockPrice
            {
                Symbol = symbol,
                CurrentPrice = todayPrice,
                PercentageChange = percentageChange
            };
        }
    }
}
