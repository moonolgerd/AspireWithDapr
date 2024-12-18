using AspireWithDapr.Shared;
using Dapr.Client;

namespace AspireWithDapr.Publisher;

public class PublisherHostedService(DaprClient daprClient) : BackgroundService
{
    private static readonly string[] Summary = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var random = Random.Shared;
            WeatherForecast weatherForecast = new() {

                Date = DateTime.Today.AddDays(random.Next(0, 6)),
                TemperatureC = random.Next(-20, 55),
                Summary = Summary[random.Next(10)],
                City = SharedCollections.Cities[random.Next(0, SharedCollections.Cities.Count)]
            };
            await daprClient.PublishEventAsync(SharedConstants.PubsubName, SharedConstants.TopicName, weatherForecast, stoppingToken);
            Console.WriteLine("Published data: " + weatherForecast);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
