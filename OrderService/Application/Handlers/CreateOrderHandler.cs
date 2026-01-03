using System.Text.Json;
using OrderService.Application.Commands;
using OrderService.Domain.Entities;
using OrderService.Domain.Events;
using OrderService.Infrastructure.Outbox;
using OrderService.Infrastructure.Persistence;

namespace OrderService.Application.Handlers;

public class CreateOrderHandler
{
    private readonly OrderDbContext _dbContext;

    public CreateOrderHandler(OrderDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> HandleAsync(CreateOrderCommand command)
    {
        var order = new Order
        {
            Id = Guid.NewGuid(),
            ProductId = command.ProductId,
            Quantity = command.Quantity,
            Price = command.Price
        };

        var orderCreatedEvent = new OrderCreatedEvent(
            order.Id,
            order.ProductId,
            order.Quantity,
            order.Price,
            order.CreatedAt
        );

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = nameof(OrderCreatedEvent),
            Payload = JsonSerializer.Serialize(orderCreatedEvent),
            OccurredOn = DateTime.UtcNow,
            Processed = false
        };

        _dbContext.Orders.Add(order);
        _dbContext.OutboxMessages.Add(outboxMessage);

        await _dbContext.SaveChangesAsync();

        return order.Id;
    }
}
