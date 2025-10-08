using AspireWithDapr.Shared;
using Dapr.Actors.Client;

namespace AspireWithDapr.ApiService;

public class Mutation
{
    public async Task<WeatherForecast> AddWeatherForecastAsync(
        WeatherForecast forecast,
        IActorProxyFactory actorProxyFactory)
    {
        var weatherActor = actorProxyFactory.CreateActorProxy<IWeatherActor>(
            new Dapr.Actors.ActorId(forecast.City), nameof(WeatherActor));
        await weatherActor.AddWeatherForecast(forecast);
        return forecast;
    }
}
