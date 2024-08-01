using AspireWithDapr.Publisher;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddDaprClient();

builder.Services.AddHostedService<PublisherHostedService>();

var app = builder.Build();

app.MapDefaultEndpoints();

app.Run();