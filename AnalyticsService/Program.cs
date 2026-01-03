using AnalyticsService.Consumers;
using AnalyticsService.Infrastructure;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<BlobStorageService>();
builder.Services.AddHostedService<OrderEventConsumer>();

builder.Services.AddSerilog(config =>
{
    config.WriteTo.Console();
});

var host = builder.Build();
host.Run();
