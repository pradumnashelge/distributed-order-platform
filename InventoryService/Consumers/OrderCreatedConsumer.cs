using MassTransit;
using Microsoft.EntityFrameworkCore;
using InventoryService.Infrastructure;

namespace InventoryService.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly InventoryDbContext _dbContext;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    public OrderCreatedConsumer(
        InventoryDbContext dbContext,
        ILogger<OrderCreatedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var message = context.Message;

        var item = await _dbContext.InventoryItems
            .FirstOrDefaultAsync(i => i.ProductId == message.ProductId);

        if (item == null || item.AvailableQuantity < message.Quantity)
        {
            _logger.LogWarning(
                "Insufficient inventory for Product {ProductId}",
                message.ProductId);

            // In real system → publish InventoryFailed event
            return;
        }

        item.AvailableQuantity -= message.Quantity;
        item.UpdatedAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation(
            "Reserved {Qty} items for Product {ProductId}",
            message.Quantity,
            message.ProductId);
    }
}
