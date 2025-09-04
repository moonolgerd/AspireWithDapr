using AspireWithDapr.Publisher;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSeqEndpoint(connectionName: "seq");

builder.Services.AddDaprClient();

builder.Services.AddHostedService<PublisherHostedService>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();