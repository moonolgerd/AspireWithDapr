using AspireWithDapr.Shared;
using Dapr.Actors;
using Dapr.Actors.Runtime;

namespace AspireWithDapr.ApiService;

/// <summary>
/// Represents an actor that provides weather forecasts.
/// </summary>
public class WeatherActor(ActorHost host) : Actor(host), IMyActor
{
    /// <summary>
    /// Gets the weather forecasts.
    /// </summary>
    /// <returns>The collection of weather forecasts.</returns>
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecasts()
    {
        var forecast = await StateManager.TryGetStateAsync<WeatherForecast[]>("forecast");
        return forecast.Value;
    }

    public async Task AddWeatherForecast(WeatherForecast forecast)
    {
        Logger.LogInformation("Adding new {forecast}", forecast);
        var existing = await StateManager.GetOrAddStateAsync<WeatherForecast[]>("forecast", [forecast]);
        
        var list = existing.ToList();
        list.Add(forecast);

        await StateManager.SetStateAsync("forecast", list.ToArray());
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

    public Task AddWeatherForecast(WeatherForecast forecast);
}
