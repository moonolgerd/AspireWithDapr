using AspireWithDapr.ApiService;
using Dapr.Actors;
using Dapr.Actors.Client;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<WeatherActor>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapGet("/weatherforecast", async (IActorProxyFactory proxy) =>
{
    var actor = proxy.CreateActorProxy<IMyActor>(new ActorId("1"), nameof(WeatherActor));
    var forecasts = await actor.GetWeatherForecasts();
    return forecasts;
});

app.MapPost("/weatherforecast", async (IActorProxyFactory proxy) =>
{
    var actor = proxy.CreateActorProxy<IMyActor>(new ActorId("1"), nameof(WeatherActor));
    Console.WriteLine("Received {actor}", await actor.GetWeatherForecasts());
}).WithTopic("pubsub", "MyTopic");

app.UseRouting();

app.MapActorsHandlers();

app.MapSubscribeHandler();

app.MapDefaultEndpoints();

app.Run();
