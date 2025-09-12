using AspireWithDapr.AppHost;
using CommunityToolkit.Aspire.Hosting.Dapr;
using Microsoft.Extensions.DependencyInjection;

var daprFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".dapr", "bin");

var builder = DistributedApplication.CreateBuilder(args);

var seq = builder.AddSeq("seq", 5341)
    .ExcludeFromManifest()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEnvironment("ACCEPT_EULA", "Y");

var redisPassword = builder.AddParameter("redis-password", "mypassword");
var cache = builder.AddRedis("Redis", port: 6379, redisPassword)
    .WithRedisInsight();

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

builder.AddExecutable("scheduler", "scheduler", daprFolder, "--port", "6060", "--metrics-port", "9092", "--healthz-port", "8089");

var localFoundry = builder.AddAzureAIFoundry("foundry")
    .RunAsFoundryLocal()
    .AddDeployment("chat", "phi-3.5-mini", "1", "Microsoft");

var apiService = builder.AddProject<Projects.AspireWithDapr_ApiService>("apiservice")
    .WithReplicas(3)
    .WithReference(cache)
    .WithReference(store)
    .WithReference(seq)
    .WithDaprSidecar("api")
    .WaitFor(seq);

builder.AddProject<Projects.AspireWithDapr_Web>("webfrontend")
    .WithDaprSidecar("web")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService)
    .WithReference(seq)
    .WithReference(localFoundry)
    .WaitFor(seq);

builder.AddProject<Projects.AspireWithDapr_Publisher>("publisher")
    .WithDaprSidecar("publisher")
    .WithExternalHttpEndpoints()
    .WithReference(pubsub)
    .WithReference(seq)
    .WaitFor(seq);

builder.Build().Run();
