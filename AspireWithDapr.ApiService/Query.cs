using AspireWithDapr.Shared;
using Dapr.Actors.Client;

namespace AspireWithDapr.ApiService
{
    public class Query
    {
        public async Task<IEnumerable<WeatherForecast>> GetWeatherForecastsAsync(IActorProxyFactory actorProxyFactory)
        {
            var weatherActor = actorProxyFactory.CreateActorProxy<IWeatherActor>(new Dapr.Actors.ActorId("1"), nameof(WeatherActor));
            return await weatherActor.GetWeatherForecasts();
        }
    }
}
