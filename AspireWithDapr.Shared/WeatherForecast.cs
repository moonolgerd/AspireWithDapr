namespace AspireWithDapr.Shared;

public record WeatherForecast
{
    public WeatherForecast()
    {
    }

    public DateTime Date { get; set; } = DateTime.Now;

    public int TemperatureC { get; set; }

    public string? Summary { get; set; }

    /// <summary>
    /// Gets the temperature in Fahrenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

public static class SharedConstants
{
    public const string PubsubName = "pubsub";
    public const string TopicName = "MyTopic";

}