using AspireWithDapr.Shared;
using Dapr.Client;

namespace AspireWithDapr.Web;

public class WeatherApiClient(DaprClient daprClient)
{
    public async Task<WeatherForecast[]> GetWeatherAsync(string city)
    {
        return await daprClient.InvokeMethodAsync<WeatherForecast[]>(HttpMethod.Get, "api", $"weatherforecast?city={city}");
    }
}
