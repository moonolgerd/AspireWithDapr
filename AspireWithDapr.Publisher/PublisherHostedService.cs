using AspireWithDapr.Shared;
using Dapr.Client;

namespace AspireWithDapr.Publisher;

public class PublisherHostedService(DaprClient daprClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var random = Random.Shared;
            WeatherForecast weatherForecast = new()
            {
                Date = DateTime.Today.AddDays(random.Next(0, 6)),
                TemperatureC = random.Next(-20, 55),
                Summary = new string[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" }[random.Next(10)]
            };
            await daprClient.PublishEventAsync(SharedConstants.PubsubName, SharedConstants.TopicName, weatherForecast, stoppingToken);
            Console.WriteLine("Published data: " + weatherForecast);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
