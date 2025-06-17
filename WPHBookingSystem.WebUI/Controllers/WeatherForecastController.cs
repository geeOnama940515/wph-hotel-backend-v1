using Microsoft.AspNetCore.Mvc;

namespace WPHBookingSystem.WebUI.Controllers
{
    /// <summary>
    /// Weather forecast controller - template controller for demonstration purposes.
    /// 
    /// This is a default ASP.NET Core Web API template controller that generates
    /// mock weather forecast data. It serves as a placeholder and can be removed
    /// or replaced with actual business functionality as needed.
    /// 
    /// The controller demonstrates basic API controller patterns including:
    /// - Dependency injection with ILogger
    /// - HTTP GET endpoint with route configuration
    /// - Returning IEnumerable of DTOs
    /// - Basic logging integration
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        /// <summary>
        /// Predefined weather summary descriptions for generating mock data.
        /// </summary>
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        /// <summary>
        /// Initializes a new instance of the WeatherForecastController with logging.
        /// </summary>
        /// <param name="logger">Logger instance for recording controller activities</param>
        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Generates mock weather forecast data for the next 5 days.
        /// 
        /// This endpoint demonstrates basic API functionality by returning
        /// randomly generated weather data. It's useful for testing API
        /// connectivity and response formats.
        /// </summary>
        /// <returns>Collection of weather forecast data for the next 5 days</returns>
        /// <response code="200">Weather forecast data generated successfully</response>
        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
