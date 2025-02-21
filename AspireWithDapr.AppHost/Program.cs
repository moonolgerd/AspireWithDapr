using Aspire.Hosting.Dapr;
using AspireWithDapr.AppHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var daprFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".dapr", "bin");

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache", "redis/redis-stack", 6379);

var store = builder.AddDaprStateStore("statestore", new DaprComponentOptions
{
    LocalPath = "../statestore.yaml"
}).WaitFor(cache);
var pubsub = builder.AddDaprPubSub("pubsub", new DaprComponentOptions
{
    LocalPath = "../pubsub.yaml"
}).WaitFor(cache);

builder.AddExecutable("dapr", "placement", daprFolder, "-port", "6050")
    .WithHttpEndpoint(6050, 6050, isProxied: false);

builder.AddExecutable("dashboard", "dapr", daprFolder, "dashboard", "-p", "9999")
    .WithHttpEndpoint(9999, targetPort: 9999, isProxied: false);

var apiService = builder.AddProject<Projects.AspireWithDapr_ApiService>("apiservice")
    .WithReplicas(3)
    .WithReference(store)
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

builder.Build().Run();
