using AspireWithDapr.Shared;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;

namespace AspireWithDapr.ApiService;

public class Subscription
{
    [Subscribe(With = nameof(SubscribeAsync))]
    public WeatherForecast OnWeatherForecast(
        [EventMessage] WeatherForecast forecast) => forecast;

    public ValueTask<ISourceStream<WeatherForecast>> SubscribeAsync(string city, ITopicEventReceiver topicEventReceiver)
    {
        return topicEventReceiver.SubscribeAsync<WeatherForecast>($"WeatherForecast:{city}");
    }
}
