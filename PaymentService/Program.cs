using MassTransit;
using Microsoft.EntityFrameworkCore;
using PaymentService.Consumers;
using PaymentService.Infrastructure;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("order-created-payment-queue", e =>
        {
            e.ConfigureConsumer<OrderCreatedConsumer>(context);

            e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        });
    });
});

builder.Services.AddSerilog(config =>
    config.WriteTo.Console()
);

var host = builder.Build();
host.Run();
