using AspireWithDapr.ApiService;
using AspireWithDapr.Shared;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

builder.AddSeqEndpoint(connectionName: "seq");

builder.AddRedisClient("Redis");

// Add services to the container.
builder.Services.AddProblemDetails();

builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<WeatherActor>();
});

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddSubscriptionType<Subscription>()
    .AddRedisSubscriptions()
    .AddInstrumentation(); 

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

app.MapGet("/weatherforecast", async ([FromQuery] string city, IActorProxyFactory proxy) =>
{
    var actor = proxy.CreateActorProxy<IWeatherActor>(new ActorId(city), nameof(WeatherActor));
    var forecasts = await actor.GetWeatherForecasts();
    return forecasts;
});

app.MapPost("/weatherforecast", async ([FromBody] WeatherForecast forecast, IActorProxyFactory proxy, ILogger<Program> logger) =>
{
    logger.LogInformation("Processing new forecast on {Id}", Environment.ProcessId);

    var actor = proxy.CreateActorProxy<IWeatherActor>(new ActorId(forecast.City), nameof(WeatherActor));
    await actor.AddWeatherForecast(forecast);

}).WithTopic("pubsub", "MyTopic");

app.UseRouting();

app.MapActorsHandlers();

app.MapSubscribeHandler();

app.UseCloudEvents();

app.UseWebSockets();

app.MapDefaultEndpoints();

app.MapGraphQL();

app.Run();
