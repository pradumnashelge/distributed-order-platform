using Microsoft.EntityFrameworkCore;
using OrderService.Application.Handlers;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Outbox;
using OrderService.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ------------------ Logging ------------------
builder.Host.UseSerilog((ctx, lc) =>
    lc.WriteTo.Console()
);

// ------------------ Controllers ------------------
builder.Services.AddControllers();

// ------------------ Database ------------------
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ------------------ Messaging Dependencies ------------------
// KafkaProducer can be singleton since the producer can be reused
builder.Services.AddSingleton<KafkaProducer>();
// MassTransitPublisher can be scoped (new instance per scope)
builder.Services.AddScoped<MassTransitPublisher>();

// ------------------ Outbox ------------------
builder.Services.AddScoped<OutboxPublisherService>();
builder.Services.AddHostedService<OutboxBackgroundService>();

// ------------------ Application Handlers ------------------
builder.Services.AddScoped<CreateOrderHandler>();

var app = builder.Build();

// ------------------ Map Controllers ------------------
app.MapControllers();

// ------------------ Run ------------------
app.Run();
