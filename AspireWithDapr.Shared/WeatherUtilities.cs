namespace AspireWithDapr.Shared;

/// <summary>
/// Utility class for weather-related operations shared across services
/// </summary>
public static class WeatherUtilities
{
    /// <summary>
    /// Gets a temperature range appropriate for the given weather summary
    /// </summary>
    /// <param name="summary">Weather summary description</param>
    /// <param name="random">Random number generator instance</param>
    /// <returns>Temperature in Celsius that matches the weather summary</returns>
    public static int GetTemperatureForSummary(string summary, Random random)
    {
        return summary switch
        {
            "Freezing" => random.Next(-20, -5),    // -20°C to -6°C
            "Bracing" => random.Next(-5, 2),      // -5°C to 1°C  
            "Chilly" => random.Next(2, 8),       // 2°C to 7°C
            "Cool" => random.Next(8, 15),        // 8°C to 14°C
            "Mild" => random.Next(15, 20),       // 15°C to 19°C
            "Warm" => random.Next(20, 25),       // 20°C to 24°C
            "Balmy" => random.Next(25, 30),      // 25°C to 29°C
            "Hot" => random.Next(30, 35),        // 30°C to 34°C
            "Sweltering" => random.Next(35, 40), // 35°C to 39°C
            "Scorching" => random.Next(40, 50),  // 40°C to 49°C
            _ => random.Next(-20, 55)             // Fallback to original range
        };
    }

    /// <summary>
    /// Determines if a weather summary indicates cold temperatures
    /// </summary>
    /// <param name="summary">Weather summary description</param>
    /// <returns>True if the weather is typically cold</returns>
    public static bool IsColdWeather(string? summary)
    {
        if (string.IsNullOrEmpty(summary))
            return false;

        return summary.Equals("Freezing", StringComparison.OrdinalIgnoreCase) ||
               summary.Equals("Bracing", StringComparison.OrdinalIgnoreCase) ||
               summary.Equals("Chilly", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines if a weather summary indicates hot temperatures
    /// </summary>
    /// <param name="summary">Weather summary description</param>
    /// <returns>True if the weather is typically hot</returns>
    public static bool IsHotWeather(string? summary)
    {
        if (string.IsNullOrEmpty(summary))
            return false;

        return summary.Equals("Hot", StringComparison.OrdinalIgnoreCase) ||
               summary.Equals("Sweltering", StringComparison.OrdinalIgnoreCase) ||
               summary.Equals("Scorching", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Gets the temperature category for a given temperature value
    /// </summary>
    /// <param name="temperatureC">Temperature in Celsius</param>
    /// <returns>Weather summary that best matches the temperature</returns>
    public static string GetSummaryForTemperature(int temperatureC)
    {
        return temperatureC switch
        {
            < -5 => "Freezing",
            >= -5 and < 2 => "Bracing",
            >= 2 and < 8 => "Chilly",
            >= 8 and < 15 => "Cool",
            >= 15 and < 20 => "Mild",
            >= 20 and < 25 => "Warm",
            >= 25 and < 30 => "Balmy",
            >= 30 and < 35 => "Hot",
            >= 35 and < 40 => "Sweltering",
            _ => "Scorching"
        };
    }
}