using Aspire.Hosting.Dapr;
using AspireWithDapr.AppHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache", "redis/redis-stack", 6379);

builder.AddDaprStateStore("statestore", new DaprComponentOptions
{
    LocalPath = $"{profile}\\.dapr\\components\\statestore.yaml"
});
var pubsub = builder.AddDaprPubSub("pubsub", new DaprComponentOptions
{
    LocalPath = $"{profile}\\.dapr\\components\\pubsub.yaml"
});

builder.AddExecutable("dapr", "placement", $"{profile}\\.dapr\\bin", "-port", "6050");

builder.AddExecutable("dashboard", "dashboard", $"{profile}\\.dapr\\bin", "-port", "9999")
    .WithHttpEndpoint(9999, targetPort: 9999, isProxied: false);

var apiService = builder.AddProject<Projects.AspireWithDapr_ApiService>("apiservice")
    .WithReplicas(3)
    .WithDaprSidecar("api");

builder.AddProject<Projects.AspireWithDapr_Web>("webfrontend")
    .WithDaprSidecar("web")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.AddProject<Projects.AspireWithDapr_Publisher>("publisher")
    .WithDaprSidecar()
    .WithExternalHttpEndpoints()
    .WithReference(pubsub);

//// Workaround for https://github.com/dotnet/aspire/issues/2219
//if (builder.Configuration.GetValue<string>("DAPR_CLI_PATH") is { } daprCliPath)
//{
//    builder.Services.Configure<DaprOptions>(options =>
//    {
//        options.DaprPath = daprCliPath;
//    });
//}

builder.Build().Run();
