using System.Numerics;
using System.Runtime.Serialization;
using AspireWithDapr.Shared;
using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace AspireWithDapr.ApiService;

/// <summary>
/// Represents an actor that provides weather forecasts.
/// </summary>
public class WeatherActor(ActorHost host) : Actor(host), IMyActor
{
    private readonly string[] summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    protected override async Task OnActivateAsync()
    {
        var forecast = Enumerable.Range(1, 5)
            .Select(index =>
            new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = summaries[Random.Shared.Next(summaries.Length)]
            })
            .ToArray();
        await StateManager.TryAddStateAsync("forecast", forecast);
    }

    /// <summary>
    /// Gets the weather forecasts.
    /// </summary>
    /// <returns>The collection of weather forecasts.</returns>
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts()
    {
        var forecast = await StateManager.TryGetStateAsync<WeatherForecast[]>("forecast");
        return forecast.Value;
    }
}

/// <summary>
/// Represents the interface for the weather forecast actor.
/// </summary>
public interface IMyActor : IActor
{
    /// <summary>
    /// Gets the weather forecasts.
    /// </summary>
    /// <returns>The collection of weather forecasts.</returns>
    public Task<IEnumerable<WeatherForecast>> GetWeatherForecasts();
}
