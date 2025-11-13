using AspireWithDapr.Web;
using AspireWithDapr.Web.Components;
using AspireWithDapr.Web.Services;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();
builder.AddRedisOutputCache("cache");

builder.AddSeqEndpoint(connectionName: "seq");

builder.AddOpenAIClient("chat")
       .AddChatClient();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDaprClient();

// Legacy Dapr-based client (for backward compatibility)
builder.Services.AddSingleton<AspireWithDapr.Web.WeatherApiClient>();
builder.Services.AddScoped<ChatService>();

// Add StrawberryShake GraphQL client
// Note: "apiservice" matches the service name in AppHost, but we use "api" for Dapr app-id
builder.Services
    .AddWeatherApiClient()
    .ConfigureHttpClient((sp, client) =>
    {
        // Aspire service discovery resolves "http://apiservice" to the actual ApiService endpoint
        // The service name "apiservice" comes from AppHost: builder.AddProject<>("apiservice")
        client.BaseAddress = new Uri("http://apiservice/graphql");
    })
    .ConfigureWebSocketClient((sp, client) =>
    {
        // Configure WebSocket for GraphQL subscriptions
        client.Uri = new Uri("ws://apiservice/graphql");
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
