namespace AspireWithDapr.AppHost;

public static class ProgramExtensions
{
    public static IResourceBuilder<RedisResource> AddRedis(this IDistributedApplicationBuilder builder, string name, string image, int? port = null)
    {
        var redis = new RedisResource(name);
        return builder.AddResource(redis)
            .WithEndpoint(port: port, targetPort: 6379, name: "tcp")
            .WithHttpEndpoint(port: 8001, targetPort: 8001)
            .WithImage(image, "latest");
    }
}
