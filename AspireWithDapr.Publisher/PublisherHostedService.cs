
using AspireWithDapr.Shared;
using Dapr.Client;

namespace AspireWithDapr.Publisher
{
    public class PublisherHostedService : BackgroundService
    {
        string PUBSUB_NAME = "pubsub";
        string TOPIC_NAME = "MyTopic";

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                System.Threading.Thread.Sleep(5000);
                Random random = new Random();
                WeatherForecast weatherForecast = new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Today),
                    TemperatureC = random.Next(-20, 55),
                    Summary = new string[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" }[random.Next(10)]
                };
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken cancellationToken = source.Token;
                using var client = new DaprClientBuilder().Build();
                //Using Dapr SDK to publish a topic
                await client.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, weatherForecast, cancellationToken);
                Console.WriteLine("Published data: " + weatherForecast);
            }
        }
    }
}
