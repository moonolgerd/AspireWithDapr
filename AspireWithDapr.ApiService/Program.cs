using AspireWithDapr.ApiService;
using AspireWithDapr.Shared;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.Mvc;

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

app.MapPost("/weatherforecast", async ([FromBody] WeatherForecast forecast, IActorProxyFactory proxy) =>
{
    var actor = proxy.CreateActorProxy<IMyActor>(new ActorId("1"), nameof(WeatherActor));
    await actor.AddWeatherForecast(forecast);

}).WithTopic("pubsub", "MyTopic");

app.UseRouting();

app.MapActorsHandlers();

app.MapSubscribeHandler();

app.UseCloudEvents();

app.MapDefaultEndpoints();

app.Run();
