using AspireWithDapr.Shared;
using Dapr.Actors;
using Dapr.Actors.Client;

namespace AspireWithDapr.ApiService;

public class Query
{
    public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync(string city, IActorProxyFactory actorProxyFactory)
    {
        var weatherActor = actorProxyFactory.CreateActorProxy<IWeatherActor>(
            new ActorId(city), nameof(WeatherActor));
        return await weatherActor.GetWeatherForecasts();
    }
}
