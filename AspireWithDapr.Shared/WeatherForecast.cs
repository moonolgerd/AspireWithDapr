namespace AspireWithDapr.Shared;

public record WeatherForecast
{
    public WeatherForecast()
    {
    }

    public DateOnly Date { get; set; } = new DateOnly();

    public int TemperatureC { get; set; }

    public string? Summary { get; set; }

    /// <summary>
    /// Gets the temperature in Fahrenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
