using Microsoft.EntityFrameworkCore;
using OrderService.Application.Handlers;
using OrderService.Infrastructure.Persistence;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) =>
    lc.WriteTo.Console()
);

builder.Services.AddControllers();

builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<CreateOrderHandler>();

var app = builder.Build();

app.MapControllers();
app.Run();
