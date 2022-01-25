using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DynamoDbDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDynamoDBContext _dbContext;


        public WeatherForecastController(ILogger<WeatherForecastController> logger, IDynamoDBContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="City"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get(string City = "Noida")
        {
            return await _dbContext
                        .QueryAsync<WeatherForecast>(City)
                        .GetRemainingAsync();
            // return GenerateDummyWeatherForcast(City);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="City"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task PostData(string City)
        {
            var Forcasts = GenerateDummyWeatherForcast(City);
            foreach (var item in Forcasts)
            {
                await _dbContext.SaveAsync(item);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task UpdateData(string city)
        {
            var getSpecificData =
                 await _dbContext.LoadAsync<WeatherForecast>(city, DateTime.UtcNow.Date.AddDays(1));
            getSpecificData.Summary = "Chilled";
            await _dbContext.SaveAsync(getSpecificData);
        }

        [HttpDelete]
        public async Task DeleteData(string city)
        {
            var getSpecificData =
                 await _dbContext.LoadAsync<WeatherForecast>(city, DateTime.UtcNow.Date.AddDays(1));
            await _dbContext.DeleteAsync(getSpecificData);
        }


        private IEnumerable<WeatherForecast> GenerateDummyWeatherForcast(string city)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                City = city,
                Date = DateTime.UtcNow.Date.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
