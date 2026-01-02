using System.Text.Json;
using Confluent.Kafka;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Serilog;
using OrderService.Application.Handlers;
using OrderService.Infrastructure.Outbox;
using OrderService.Infrastructure.Persistence;
using OrderService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Configuration
var configuration = builder.Configuration;
configuration.AddInMemoryCollection(new Dictionary<string, string?>
{
    ["ConnectionStrings:Sqlite"] = "Data Source=orders.db",
    ["Kafka:BootstrapServers"] = "localhost:9092",
    ["RabbitMq:Host"] = "rabbitmq://localhost",
    ["RabbitMq:Username"] = "guest",
    ["RabbitMq:Password"] = "guest"
});

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure registrations (DbContext, MassTransit, Kafka producer, Outbox publisher)
builder.Services.AddInfrastructure(configuration);

// Application handlers
builder.Services.AddScoped<CreateOrderHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.MapControllers();

// Start background outbox publisher
var outbox = app.Services.GetRequiredService<IOutboxPublisher>();
outbox.StartPublishing(app.Lifetime.ApplicationStopping);

app.Run();
