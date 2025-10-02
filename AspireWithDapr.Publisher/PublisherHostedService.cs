using AspireWithDapr.Shared;
using Dapr.Client;

namespace AspireWithDapr.Publisher;

public class PublisherHostedService(DaprClient daprClient, ILogger<PublisherHostedService> logger) : BackgroundService
{
    private static readonly string[] Summary = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var random = Random.Shared;
            var summaryIndex = random.Next(10);
            var summary = Summary[summaryIndex];
            
            WeatherForecast weatherForecast = new() {
                Date = DateTime.Today.AddDays(random.Next(0, 6)),
                TemperatureC = WeatherUtilities.GetTemperatureForSummary(summary, random),
                Summary = summary,
                City = SharedCollections.Cities[random.Next(0, SharedCollections.Cities.Count)]
            };
            
            await daprClient.PublishEventAsync(SharedConstants.PubsubName, SharedConstants.TopicName, weatherForecast, stoppingToken);
            logger.LogInformation("Published data: {weatherForecast}", weatherForecast);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
