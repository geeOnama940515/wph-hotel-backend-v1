namespace WPHBookingSystem.WebUI
{
    /// <summary>
    /// Data transfer object for weather forecast information.
    /// 
    /// This class represents weather forecast data used by the WeatherForecastController.
    /// It's a template DTO that demonstrates basic data transfer object patterns
    /// including property getters/setters and computed properties.
    /// 
    /// The class includes:
    /// - Date for the forecast
    /// - Temperature in Celsius
    /// - Computed temperature in Fahrenheit
    /// - Weather summary description
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// The date for which this weather forecast applies.
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Temperature in degrees Celsius.
        /// </summary>
        public int TemperatureC { get; set; }

        /// <summary>
        /// Temperature in degrees Fahrenheit, computed from Celsius.
        /// 
        /// Formula: F = 32 + (C / 0.5556)
        /// This is a computed property that automatically converts Celsius to Fahrenheit.
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        /// <summary>
        /// Textual description of the weather conditions.
        /// 
        /// Examples: "Sunny", "Cloudy", "Rainy", etc.
        /// Can be null if no summary is available.
        /// </summary>
        public string? Summary { get; set; }
    }
}
