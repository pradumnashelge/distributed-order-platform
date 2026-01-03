using MassTransit;
using Microsoft.EntityFrameworkCore;
using InventoryService.Consumers;
using InventoryService.Infrastructure;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SqlServer")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<OrderCreatedConsumer>();
    x.AddConsumer<OrderCancelledConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("order-created-inventory-queue", e =>
        {
            e.ConfigureConsumer<OrderCreatedConsumer>(context);
        });

        cfg.ReceiveEndpoint("order-cancelled-inventory-queue", e =>
        {
            e.ConfigureConsumer<OrderCancelledConsumer>(context);
        });
    });
});

builder.Services.AddSerilog(c => c.WriteTo.Console());

var host = builder.Build();
host.Run();
