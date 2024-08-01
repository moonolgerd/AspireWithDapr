using Aspire.Hosting.Dapr;
using AspireWithDapr.AppHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache", "redis/redis-stack", 6379);

builder.AddDaprStateStore("statestore", new DaprComponentOptions
{
    LocalPath = "C:\\Users\\Oleg\\.dapr\\components\\statestore.yaml"
});
builder.AddDaprPubSub("pubsub", new DaprComponentOptions
{
    LocalPath = "C:\\Users\\Oleg\\.dapr\\components\\pubsub.yaml"
});

builder.AddExecutable("dapr", "placement", "C:\\Users\\Oleg\\.dapr\\bin", "-port", "6050");

var apiService = builder.AddProject<Projects.AspireWithDapr_ApiService>("apiservice")
    .WithDaprSidecar("api");

builder.AddProject<Projects.AspireWithDapr_Web>("webfrontend")
    .WithDaprSidecar("web")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

// Workaround for https://github.com/dotnet/aspire/issues/2219
if (builder.Configuration.GetValue<string>("DAPR_CLI_PATH") is { } daprCliPath)
{
    builder.Services.Configure<DaprOptions>(options =>
    {
        options.DaprPath = daprCliPath;
    });
}

builder.Build().Run();
