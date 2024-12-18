namespace AspireWithDapr.Shared;

public record WeatherForecast
{
    public WeatherForecast()
    {
    }

    public DateTime Date { get; init; } = DateTime.Now;

    public int TemperatureC { get; init; }

    public string? Summary { get; init; }

    public string City { get; init; } = "";

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

public static class SharedCollections
{
    public static readonly List<string> Cities = 
        [
        "Athens", 
        "Berlin",
        "Cairo",
        "Damask",
        "Freetown", 
        "Gibraltar", 
        "Hanoi", 
        "Islamabad", 
        "Jakarta", 
        "Kyiv", 
        "London", 
        "Moscow",
        "Nairobi", 
        "Oslo", 
        "Prague", 
        "Quito", 
        "Rome", 
        "Stokholm", 
        "Tripoli", 
        "Ulaanbaatar", 
        "Vienna", 
        "Washington", 
        "Yerevan", 
        "Zagreb"
        ];
}